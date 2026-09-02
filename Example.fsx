#load "./ValidationLibrary.fsx"
open ValidationLibrary
open System

//Define types for PersonInput and ValidPerson
type PersonInput = { Name: string option; DOB: DateTime option; Appointment: DateTime option }
type ValidPerson = { Name: string; DOB: DateTime; Appointment: DateTime }

//Define rules for validation
let nameRule : Rule<string> = {
    Condition = fun name -> name <> ""
    ErrorMessage = "Name cannot be empty"
}

let nameRule2 : Rule<string> = {
    Condition = fun name -> name.Length >= 2
    ErrorMessage = "Name must be at least 2 characters long"
}

let dobRule : Rule<DateTime> = {
    Condition = fun dob -> dob <> DateTime.MinValue
    ErrorMessage = "Date of birth cannot be empty"
}

let dobRule2 : Rule<DateTime> = {
    Condition = fun dob -> dob < System.DateTime.Now
    ErrorMessage = "Date of birth must be in the past"
}

let dobRule3 : Rule<DateTime> = {
    Condition = fun dob -> dob > System.DateTime.Now.AddYears(-120)
    ErrorMessage = "Date of birth must be within the last 120 years"
}

let appointmentTimeRule : Rule<DateTime> = {
    Condition = fun appointmentTime -> appointmentTime <> DateTime.MinValue
    ErrorMessage = "Appointment time cannot be empty"
}

let appointmentTimeRule2 : Rule<DateTime> = {
    Condition = fun appointmentTime -> appointmentTime > System.DateTime.Now
    ErrorMessage = "Appointment time must be in the future"
}

//Define types for validation results
type NameValidations = NameMustBeEntered | TooShort
type DOBValidations =  DOBMustBeEntered | FutureDate | Before1906
type AppointmentValidations = AppointmentMustBeEntered | PastDate

type PersonValidations = 
| Name of NameValidations
| DOB of DOBValidations
| Appointment of AppointmentValidations

//Functions to check each field and return a list of validation errors
let checkName (name: string option) : NameValidations list =
    match name with
    | None -> [NameMustBeEntered]
    | Some n -> validateField id [nameRule2] (fun _ -> TooShort) n

let checkDOB (dob: DateTime option) : DOBValidations list =
    match dob with
    | None -> [DOBMustBeEntered]
    | Some d -> 
        (validateField id [dobRule2] (fun _ -> FutureDate) d) @
        (validateField id [dobRule3] (fun _ -> Before1906) d)

let checkAppointment (appointment: DateTime option) : AppointmentValidations list =
    match appointment with
    | None -> [AppointmentMustBeEntered]
    | Some a -> validateField id [appointmentTimeRule2] (fun _ -> PastDate) a

//Function to validate a PersonInput and return a ValidationResult
let validate (input: PersonInput) : ValidationResult<ValidPerson, PersonValidations> =
    let nameErrors = checkName input.Name |> List.map Name
    let dobErrors = checkDOB input.DOB |> List.map DOB
    let appointmentErrors = checkAppointment input.Appointment |> List.map Appointment

    let allErrors = nameErrors @ dobErrors @ appointmentErrors
    if List.isEmpty allErrors then
        OK { Name = input.Name.Value; DOB = input.DOB.Value; Appointment = input.Appointment.Value }
    else
        Error allErrors

let testInputOK : PersonInput = { Name = Some "Aleesha"; DOB = Some (DateTime(2002, 11, 14)); Appointment = Some (DateTime.Now.AddDays(1.0)) }
let testInputError : PersonInput = { Name = Some "A"; DOB = Some (DateTime(1500, 1, 1)); Appointment = Some (DateTime.Now.AddDays(-10.0)) }

let resultFailure = validate testInputError
let resultSuccess = validate testInputOK

printfn "%A" resultFailure 
printfn "%A" resultSuccess