module Program

open System
open System.Runtime.Serialization
open System.IO
open System.Text

let writeUsing (f : Stream -> Stream) = 

    let stream = f (new MemoryStream ())
    stream.Position <- 0L

    use reader = new StreamReader (stream)
    reader.ReadToEnd ()

let readUsing (xml : String) f = 

    let bytes = Encoding.UTF8.GetBytes (xml)
    use stream = new MemoryStream (bytes)

    let developer = f stream

    developer

let writeStandard (developer : Developer) = 
    writeUsing (fun stream ->

        let serialiser = DataContractSerializer (typeof<Developer>)
        serialiser.WriteObject (stream, developer)

        stream
    )

let readStandard xml =
    readUsing xml (fun stream ->

        let serialiser = DataContractSerializer (typeof<Developer>)
        serialiser.ReadObject (stream) :?> Developer

    )

let writeSurrogate (developer : Developer) = 
    writeUsing (fun stream ->

        let surrogate = Surrogate.get ()
        let serialiser = DataContractSerializer (typeof<Developer>, Seq.empty, Int32.MaxValue, true, false, surrogate)
        serialiser.WriteObject (stream, developer)

        stream
    )

let readSurrogate xml =
    readUsing xml (fun stream ->

        let surrogate = Surrogate.get ()
        let serialiser = DataContractSerializer (typeof<Developer>, Seq.empty, Int32.MaxValue, true, false, surrogate)
        serialiser.ReadObject (stream) :?> Developer

    )

let writeTransform (developer : Developer) = 
    writeUsing (fun stream ->

        let surrogate = Surrogate.get ()
        let serialiser = DataContractSerializer (typeof<Developer>, Seq.empty, Int32.MaxValue, true, false, surrogate)
        serialiser.WriteObject (stream, developer)

        CleanUp.apply stream
    )

[<EntryPoint>]
let main _ = 
    
    ///Uncomment as appropriate
    //let write, read = writeStandard, readStandard //Default
    //let write, read = writeSurrogate, readSurrogate //Surrogate only
    let write, read = writeTransform, readSurrogate //Surrogate and transform (transform only required for writes)

    (* NOTE Add EmitDefaultValue = false to DataMember attributes on the model to omit empty elements from the XML. *)
        
    //Change optional values here
    let dept = Some { Department.Name = "Product Development"; }
    let team = Some { Name = "Red Team"; Department = dept; }
    let developer = { Name = "John Smith"; Team = team; }
    
    let xml = write developer
    let developer' = read xml

    ///NOTE That printfn writes None as "null".

    printfn "Original record"
    printfn "-----------------"
    printfn "%A" developer
    printfn ""

    printfn "Serialized as XML"
    printfn "-----------------"
    printfn "%s" xml
    printfn ""

    printfn "Record parsed from XML"
    printfn "-----------------"
    printfn "%A" developer'

    Console.ReadLine ()
    |> ignore

    0 
