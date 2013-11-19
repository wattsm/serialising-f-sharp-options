[<RequireQualifiedAccess>]
module CleanUp 

    open System
    open System.Xml
    open System.IO

    let [<Literal>] XmlnsNamespace = "http://www.w3.org/2000/xmlns/"
    let [<Literal>] SchemaNamespace = "http://www.w3.org/2001/XMLSchema-instance"

    let private getXml (stream : Stream) = 

        stream.Position <- 0L

        let doc = XmlDocument ()
        doc.Load (stream)

        doc

    let apply (input : Stream) = 

        let rec removeAttrs (node : XmlNode) = 

            if (node.Attributes <> null && node.Attributes.Count > 0) then
                node.Attributes
                |> castMany<XmlAttribute>
                |> List.filter (fun attr -> not (attr.NamespaceURI = SchemaNamespace))
                |> List.iter (fun attr -> node.Attributes.Remove (attr) |> ignore)

            if node.HasChildNodes then
                node.ChildNodes
                |> castMany<XmlNode>
                |> Seq.iter removeAttrs

        let appendXmlns (xml : XmlDocument) = 

            let attr = xml.CreateAttribute ("xmlns", "i", XmlnsNamespace)
            attr.Value <- SchemaNamespace

            xml.DocumentElement.Attributes.Append (attr)
            |> ignore

        let xml = getXml input

        removeAttrs xml.DocumentElement
        appendXmlns xml

        let settings = XmlWriterSettings ()
        settings.OmitXmlDeclaration <- true
        settings.NamespaceHandling <- NamespaceHandling.OmitDuplicates

        let output = new MemoryStream ()
        use writer = XmlWriter.Create (output, settings)

        xml.WriteTo (writer)

        output :> Stream
