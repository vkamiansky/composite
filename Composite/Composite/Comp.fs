namespace Composite

open FSharp.Core

[<RequireQualifiedAccess>]
module Comp =

    ///<summary>Returns the sequence of inner components for composite <c>source</c> or a sequence with <c>source</c> as one element for value <c>source</c>.</summary>
    ///<param name="source">The input <c>Composite</c>.</param>
    let inner (source: 'T Composite) =
        match source with
        | Composite x -> x
        | x -> seq { yield x }

    ///<summary>Unfolds the input sequence according to the given scenario.</summary>
    ///<param name="scenario">An array of check and unfold rules. The rules are applied according to their order in the scenario. Each rule is not applied to the results of its application. The rule consists of a check function determining whether the element can be unfolded and an unfold function defining the unfold of an element into a sequence.</param>
    ///<param name="source">The input <c>Composite</c>.</param>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>scenario</c> is null.</exception>
    ///<exception cref="System.ArgumentException">Thrown when <c>scenario</c> contains no unfold rules.</exception>
    let unfold (scenario: (('T -> bool) * ('T -> seq<'T>))[]) source =
        if scenario |> isNull then nullArg "scenario"
        if scenario |> Array.isEmpty then invalidArg "scenario" "Unfold scenario must contain at least one step."

        // Applies a check/unfold rule to the given object.
        // If the object passes the check function it is unfolded
        // through the use of the unfold function, the results are wrapped
        // into Values and their enumerable - into a Composite.
        // If the object does not pass the check function it
        // is returned as is wrapped in a Value.
        let applyRuleAndWrapResult rule obj =
            let (checkFunc, unfoldFunc) = rule
            obj
            |> checkFunc
            |> function
                | true -> obj |> unfoldFunc |> Seq.map Value |> Composite
                | _ -> obj |> Value

        // We pass through a sequence and apply the unfold
        // steps to each object. Each step produces a sequence of
        // results we pack into a new composite of values.
        let rec getResults steps comp =
            steps
            |> Array.tryHead
            |> function
                | None -> comp
                | Some step -> comp
                                |> function
                                   | Value x -> getResults (steps |> Array.tail) (x |> applyRuleAndWrapResult step)
                                   | Composite x -> if x |> isNull then failwith "Composite sequence must not be null."
                                                    x |> Seq.map (getResults steps) |> Composite
        
        getResults scenario source