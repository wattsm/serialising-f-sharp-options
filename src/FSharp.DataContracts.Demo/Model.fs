///Contains the data model used by the demo
[<AutoOpen>]
module Model

open System
open System.Runtime.Serialization

[<DataContract (Name = "department", Namespace = "")>]
type Department = {
    [<field: DataMember (Name = "name")>] Name : String;
}

[<DataContract (Name = "team", Namespace = "")>]
type Team = {
    [<field: DataMember (Name = "name")>] Name : String;
    [<field: DataMember (Name = "dept", EmitDefaultValue = false)>] Department : Department option;
}

[<DataContract (Name = "developer", Namespace = "")>]
type Developer = {
    [<field: DataMember (Name = "name")>] Name : String;
    [<field: DataMember (Name = "team", EmitDefaultValue = true)>] Team : Team option;
}