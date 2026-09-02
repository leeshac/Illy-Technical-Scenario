This is a small library, written in F#, for defining validations for a generic type.

### Files

- `ValidationLibrary.fsx` – The generic validation library
- `Example.fsx` – An example showing the library in use, validating Name, DOB, and Appointment

### How to run

Run the example script with:

```
dotnet fsi Example.fsx
```

This runs the two sample inputs, one that passes validation and one that fails, and prints both results:

```fsharp
let testInputError : PersonInput = { Name = Some "A"; DOB = Some (DateTime(1500, 1, 1)); Appointment = Some (DateTime.Now.AddDays(-10.0)) }
let testInputOK : PersonInput = { Name = Some "Aleesha"; DOB = Some (DateTime(2002, 11, 14)); Appointment = Some (DateTime.Now.AddDays(1.0)) }

let resultFailure = validate testInputError
let resultSuccess = validate testInputOK

printfn "%A" resultFailure 
printfn "%A" resultSuccess
```

### Example output

```
Error [Name TooShort; DOB Before1906; Appointment PastDate]
OK { Name = "Aleesha"
     DOB = 14/11/2002 00:00:00
     Appointment = 03/09/2026 17:19:13 }
```
