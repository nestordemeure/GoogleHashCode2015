module GHC.Domain

open ExtCore.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.Common

//-------------------------------------------------------------------------------------------------

type Capa = { garan : int ; local : int array array}

let emptyCapa poolNum rowNum =
   let loc = Array.init poolNum (fun _ -> Array.create rowNum 0)
   {garan = 0 ; local = loc}

let updateCapa row pool capaServer capa =
   let loc = Array.init capa.local.Length (fun p -> Array.copy capa.local.[p])
   loc.[pool].[row] <- loc.[pool].[row] + capaServer
   let mutable garan = System.Int32.MaxValue
   for p = 0 to capa.local.Length - 1 do 
      let poolCapa = (Array.sum capa.local.[p]) - (Array.max capa.local.[p])
      garan <- min garan poolCapa
   {garan = garan ; local = loc}

//-------------------------------------------------------------------------------------------------

type Server = { size : int ; capa : int ; id : int ; pool : int }

type Slots = { index : int ; length : int ; serveurs : Server list }
type Row = Slots list

//-------------------------------------------------------------------------------------------------

type Alloc = { row : int ; slot : int ; pool : int }
type Solution = (Alloc option) array
