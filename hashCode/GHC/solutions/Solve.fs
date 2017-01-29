module GHC.Solve

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

type State = {fullSize : int ; garantedCapa : int ; rows : Row list}

//-------------------------------------------------------------------------------------------------

//let newSt = {weight=st.weight+ob.weight; price=st.price+ob.price; objects= ob :: st.objects}

let buildStates server state poolNum = 
   let state = {state with fullSize = state.fullSize + server.size}

   // find the slot in which the serveur fits best and insert into it for all pools
   // slots are in the same row : they are equivalent
   // retrurns a (pool*row) list
   let insertInRow row =
      let bestSlot = List.minBy (fun s -> if s.length < server.size then Int.maxValue else s.length - server.size) row
      if bestSlot.length < server.size then [] else 
         let rec ins row = 
            match row with 
            | [] -> row 
            | slot::q when slot.index <> bestSlot.index -> 
               ins q |> List.map (fun ro -> slot::ro)
            | slot::q -> 
               let newSlot = {slot with length = slot.length - server.size}
               [
                  for p = 0 to poolNum - 1 do
                     yield ( p, {newSlot with serveurs = {server with pool=p}::slot.serveurs} )
               ] |> List.map (fun (p,s) -> p,s::q)
         ins row      

   let rec insertInRows i rows =
      match rows with 
      | [] -> rows 
      | row::q -> 
         let localNewRows = insertInRow row |> List.map (fun (p,r) -> i,p,r::q)
         let newRows = insertInRows (i+1) q |> List.map (fun (i,p,r) -> i,p,row::r)
         List.append localNewRows newRows

   let newRows = insertInRows 0 state.rows

   [
      for (r,p,rows) in newRows do 
         yield {state with rows = rows} //TODO update garanteed capa
   ]

//-------------------------------------------------------------------------------------------------
// SOLUTION

/// solution
let solution rows serveurs poolNum =
   let emptyState = {fullSize=0;garantedCapa=0;rows=List.ofArray rows}
   let mutable states = Map.singleton 0 emptyState
   let mutable oldStates = states

   let addServeur (se:Server) state = 
      let newStates = buildStates se state poolNum

      for newSt in newStates do
         match Map.tryFind newSt.fullSize oldStates with 
         | Some oldSt when oldSt.garantedCapa >= newSt.garantedCapa -> ()
         | _ -> oldStates <- Map.add newSt.fullSize newSt oldStates

  /// update the states
   for se in serveurs do 
      states <- oldStates
      for st in states do
         addServeur se st

  // maxBy garantedCapa
   Map.fold (fun acc _ st -> if st.garantedCapa>acc.garantedCapa then st else acc) emptyState oldStates

