module GHC.Import

open System.IO
open ExtCore.Collections
open ExtCore.IO

open GHC.Extensions.Common
open GHC.Extensions.Scanf
open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------

/// take a slotIndex and a row, 
/// output a copy of the row in which the interval containing the index has been cut into two parts (at most)
let rec killSlot slotIndex (row : Row) =
   match row with 
   | [] -> row 
   | s::q when (s.index + s.length - 1 < slotIndex) || (s.index > slotIndex) -> 
      s :: (killSlot slotIndex q)
   | s::q -> 
      let s1 = {index = s.index; length = slotIndex - s.index; servers = [] }
      let s2 = {index = slotIndex + 1; length = s.length - s1.length - 1; servers = [] }
      match s1.length=0, s2.length=0 with 
      | true, true -> q
      | true, false -> s2::q 
      | false,true -> s1::q
      | false, false -> s1::s2::q

//-------------------------------------------------------------------------------------------------
// IMPORT

let import path =
   let text = File.ReadAllLines(path)
   let (rowNum,slotNum,deadSlotNum,poolNum,serverNum) = sscanf "%d %d %d %d %d" text.[0]
   // each row starts with a single interval that will be divided as the deadslot are taken into account 
   let rows = Array.create rowNum [{index=0 ; length=slotNum ; servers=[]}]
   for u = 1 to deadSlotNum do 
      let row,slot = sscanf "%d %d" text.[u]
      rows.[row] <- killSlot slot rows.[row]
   // serveurs are build one after the other
   let serveurs =
      [|
         for s = (deadSlotNum+1) to (deadSlotNum+1) + (serverNum-1) do
            let size,capa = sscanf "%d %d" text.[s]
            let id = s - (deadSlotNum+1)
            yield {id = id; size = size; capa = capa; pool = -1; row = -1 ; slot = -1}
      |]
   rows,serveurs,poolNum