[<RequireQualifiedAccess>]
module Surrogate

    open System
    open System.Runtime.Serialization
    open System.Reflection    

    ///Surrogate which handles the conversion to/from optional types during serialisation
    type private OptionSurrogate () = 

        interface IDataContractSurrogate with

            ///Serialiser Option<'T> as 'T
            member this.GetDataContractType (currentType : Type) = 
                match (getOptionalType currentType) with
                | Some baseType -> baseType
                | _ -> currentType

            ///Convert null to None and 'T to Some<'T>
            member this.GetDeserializedObject (current, currentType) = 
                if (isOptionalType currentType) then
                    if (current <> null) then
                        Activator.CreateInstance (currentType, [| current |])
                    else
                    
                        let noneProperty = currentType.GetProperty ("None")
                        noneProperty.GetValue (null)

                else
                    current

            ///Convert Some<'T> to 'T and None to null
            member this.GetObjectToSerialize (current, _) = 
                if (current <> null) then
                
                    let currentType = current.GetType ()

                    if (isOptionalType currentType) then

                        let isSomeProperty = currentType.GetProperty "IsSome"
                        let isSome = isSomeProperty.GetValue (null, [| current |]) :?> bool

                        if isSome then

                            let valueProperty = currentType.GetProperty "Value"
                            valueProperty.GetValue (current)

                        else
                            null
                    else
                        current
                else
                    current

            member this.GetCustomDataToExport (_ : MemberInfo, _ : Type) = box null
            member this.GetCustomDataToExport (_ : Type, _ : Type) = box null
            member this.GetKnownCustomDataTypes _ = ()
            member this.ProcessImportedType (decl, _) = decl
            member this.GetReferencedTypeOnImport (_, _, _) = null

    ///Gets an optional surrogate
    let get () = 
        OptionSurrogate () :> IDataContractSurrogate