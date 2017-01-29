module GHC.Domain

open ExtCore.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.Common

//-------------------------------------------------------------------------------------------------

//type graph = Dictionary<'key,'Node>

type Server = { size : int ; capa : int ; id : int ; pool : int }

type Slots = { index : int ; length : int ; serveurs : Server list }
type Row = Slots list

//-------------------------------------------------------------------------------------------------

type Alloc = { row : int ; slot : int ; pool : int }
type Solution = (Alloc option) array
