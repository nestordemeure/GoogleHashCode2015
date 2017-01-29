module GHC.Domain

open ExtCore.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.Common

//-------------------------------------------------------------------------------------------------

type Capa = { garan : int ; local : int array array ; full : int ; upd : int}

let emptyCapa poolNum rowNum =
   let loc = Array.init poolNum (fun _ -> Array.create rowNum 0)
   {garan = 0 ; local = loc ; full = 0 ; upd = 0}

let updateCapa row pool capaServer capa =
   let loc = Array.init capa.local.Length (fun p -> Array.copy capa.local.[p])
   loc.[pool].[row] <- loc.[pool].[row] + capaServer
   let mutable garan = System.Int32.MaxValue
   for p = 0 to capa.local.Length - 1 do 
      let poolCapa = (Array.sum loc.[p]) - (Array.max loc.[p])
      garan <- min garan poolCapa
   {garan = garan ; local = loc ; full = capa.full + capaServer ; upd = capa.upd + 1}

//-------------------------------------------------------------------------------------------------

type Server = { size : int ; capa : int ; id : int ; pool : int }

type Interval = { index : int ; length : int ; servers : Server list }

type Row = Interval list

