module GHC.SolveBugged

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

type State = {fullSize : int ; garantedCapa : Capa ; rows : Row list}

//-------------------------------------------------------------------------------------------------


//-------------------------------------------------------------------------------------------------

let buildStates server state poolNum = 
   let state = {state with fullSize = state.fullSize + server.size}

   // find the slot in which the serveur fits best and insert into it for all pools
   // slots are in the same row : they are equivalent
   // returns a (pool*row) list
   let insertInRow row =
      let bestSlot = List.minBy (fun s -> if s.length < server.size then System.Int32.MaxValue else s.length - server.size) row
      if bestSlot.length < server.size then [] else 
         let rec ins row = 
            match row with 
            | [] -> [] 
            | slot::q when slot.index <> bestSlot.index -> 
               ins q |> List.map (fun (p,ro) -> p,slot::ro)
            | slot::q -> 
               let newSlot = {slot with length = slot.length - server.size}
               [
                  for p = 0 to poolNum - 1 do
                     yield ( p, {newSlot with serveurs = {server with pool=p}::slot.serveurs}::q )
               ]
         ins row      

   let rec insertInRows i rows =
      match rows with 
      | [] -> [] 
      | row::q -> 
         let localNewRows = insertInRow row |> List.map (fun (p,r) -> i,p,r::q)
         let newRows = insertInRows (i+1) q |> List.map (fun (j,p,r) -> j,p,row::r)
         List.append localNewRows newRows

   let newRows = insertInRows 0 state.rows

   [
      for (r,p,rows) in newRows do 
         yield {state with rows = rows ; garantedCapa = updateCapa r p server.capa state.garantedCapa}
   ]

//-------------------------------------------------------------------------------------------------
// SOLUTION

/// solution
let solution rows (serveurs:Server array) poolNum =
   let emptyState = {fullSize=0;rows=List.ofArray rows;garantedCapa=emptyCapa poolNum rows.Length}
   let mutable states = Map.singleton 0 emptyState
   let mutable oldStates = states

   let mutable result = emptyState

   let addServeur (se:Server) state = 
      let newStates = buildStates se state poolNum

      for newSt in newStates do
         match Map.tryFind newSt.fullSize oldStates with 
         | Some oldSt when (oldSt.garantedCapa.garan,oldSt.garantedCapa.full) > (newSt.garantedCapa.garan,newSt.garantedCapa.full) -> ()
         | _ -> 
            oldStates <- Map.add newSt.fullSize newSt oldStates
            if (newSt.garantedCapa.garan,newSt.garantedCapa.full) > (result.garantedCapa.garan,result.garantedCapa.full) then
                  result <- newSt

  /// update the states
   for se in serveurs do 
      states <- oldStates
      printf "se : %d/%d  " se.id serveurs.Length
      for st in states do
         addServeur se st.Value
      printfn "best capa : %d" result.garantedCapa.garan

   result

