// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;

namespace System.CommandLine.Validation;

public class RangeValidator : ValueValidator
{
    public RangeValidator() : base("Range")
    { }

    public override IEnumerable<ParseError>? Validate(object? value,
        CliValueResult valueResult, ValueCondition valueCondition, ValidationContext context)
    {

        List<ParseError>? parseErrors = null;
        var dataSymbol = valueResult.ValueSymbol;
        var range = GetTypedValueConditionOrThrow<Range>(valueCondition);
        var comparableValue = GetValueAsTypeOrThrow<IComparable>(value);

        // TODO: Replace the strings we are comparing with a diagnostic ID when we update ParseError
        if (range.LowerBound is not null)
        {
            if (comparableValue.CompareTo(range.LowerBound) <= 0)
            {
                AddValidationError(ref parseErrors, $"The value for '{dataSymbol.Name}' is below the lower bound of {range.LowerBound}", []);
            }
        }

        if (range.UpperBound is not null)
        {
            if (comparableValue.CompareTo(range.UpperBound) >= 0)
            {
                AddValidationError(ref parseErrors, $"The value for '{dataSymbol.Name}' is above the upper bound of {range.UpperBound}", []);
            }
        }
        return parseErrors;
    }


}
