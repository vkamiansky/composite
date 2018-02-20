namespace Composite

open System
open System.Collections.Generic

    type CheckTransformPair<'a, 'b>(checkFunctions, transformFunction) =

        member this.CheckFunctions: Func<'a, bool>[] = checkFunctions

        member this.TransformFunction: Func<'a[], IEnumerable<'b>> = transformFunction
