namespace GHC.Extensions

open System
//open System.Collections.Generic // for Dictionary
//open FSharp.Collections.ParallelSeq // for PSeq
//open System.Threading.Tasks // for Parallel.ForEach(data, action) |> ignore

//-------------------------------------------------------------------------------------------------
// FUNCTIONS

module Common =
    /// unit pipe
    let inline (|->) x f = f x ; x

    /// let timeout = timeoutIn 1000
    /// if timeout () then (*something*) else (*something else*)
    let timeoutIn milliseconds =
        let timer = Diagnostics.Stopwatch.StartNew()
        fun () -> timer.ElapsedMilliseconds <= milliseconds

//-------------------------------------------------------------------------------------------------
// ARRAY

module Array =
   /// swap the values at the given indexes
   let inline swap (a : 'T array) i j =
      let temp = a.[i]
      a.[i] <- a.[j]
      a.[j] <- temp

   let existsi predicate (a : 'T array) =
      let mutable i = 0
      let mutable doesExists = false
      while (i < a.Length) && (not doesExists) do 
         doesExists <- predicate i a.[i]
         i <- i+1
      doesExists

//-------------------------------------------------------------------------------------------------

