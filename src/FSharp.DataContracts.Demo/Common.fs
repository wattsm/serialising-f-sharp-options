///Contains common helper functions
[<AutoOpen>]
module Common

open System
open System.Linq

///Casts an enumerable sequence to a given type
let castMany<'T> (collection : System.Collections.IEnumerable) = 
    collection.Cast<'T> ()
    |> Seq.toList //Force evaluation

///Gets the base type from an optional type
let getOptionalType (x : Type) = 
    if (x.IsGenericType && x.GetGenericTypeDefinition () = typedefof<Option<_>>) then
        let args = x.GetGenericArguments ()
        in Some args.[0]
    else
        None

///True if a type is optional
let isOptionalType x = 
    Option.isSome (getOptionalType x)
    

