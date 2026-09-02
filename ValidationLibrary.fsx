//Define a type for validation rules
type Rule<'a> = {
    Condition: 'a -> bool
    ErrorMessage: string
}

//Function to run a rule with a given value, if the condition is met, it returns None, otherwise it returns an error message
let runRule (rule: Rule<'a>) (value: 'a) = 
    if rule.Condition value then None else Some rule.ErrorMessage

//Function to validate a field against a list of rules and return a list of error messages
let validateField (getField: 'a -> 'b) (rules: Rule<'b> list) (mapError: string -> 'err) (record: 'a) : 'err list =
    let value = getField record
    rules 
    |> List.choose (fun rule -> runRule rule value)
    |> List.map mapError

//Define a type for validation results
type ValidationResult<'t, 'err> = OK of 't | Error of 'err list