module GHC.Import

open System.IO
open ExtCore.Collections
open ExtCore.IO

open GHC.Extensions.Common
open GHC.Extensions.Scanf
open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------

let rec killSlot slot (row : Row) =
   match row with 
   | [] -> 
      row 
   | s::q when (s.index + s.length - 1 < slot) || (s.index > slot) -> 
      s :: (killSlot slot q)
   | s::q -> 
      let s1 = {index=s.index ; length = slot-s.index ; serveurs=[]}
      let s2 = {index=slot+1 ; length = s.length - s1.length - 1 ; serveurs=[]}
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

   let rows = Array.create rowNum [{index=0 ; length=slotNum ; serveurs=[]}]
   for u = 1 to deadSlotNum do 
      let row,slot = sscanf "%d %d" text.[u]
      rows.[row] <- killSlot slot rows.[row]
      
   let serveurs =
      [|
         for s = deadSlotNum+1 to deadSlotNum+1+serverNum-1 do
            let size,capa = sscanf "%d %d" text.[s]
            let id = s-deadSlotNum+1
            yield {size=size;capa=capa;id=id;pool= -1}
      |]

   rows,serveurs,poolNum