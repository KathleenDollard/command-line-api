// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;

namespace System.CommandLine.Validation;

public abstract class ValueValidator : Validator
{
    protected ValueValidator(string name, params Type[] valueConditionTypes)
        : base(name, valueConditionTypes)
    { }

    // These methods provide consistent messages
    protected TDataValueCondition GetTypedValueConditionOrThrow<TDataValueCondition>(ValueCondition valueCondition)
        where TDataValueCondition : ValueCondition
        => valueCondition is TDataValueCondition typedValueCondition
            ? typedValueCondition
            : throw new ArgumentException($"{Name} validation failed to find bounds");

    protected TValue GetValueAsTypeOrThrow<TValue>(object? value)
        => value is TValue typedValue
            ? typedValue
            : throw new InvalidOperationException($"{Name} validation does not apply to this type");

    public abstract IEnumerable<ParseError>? Validate(object? value, CliValueResult? valueResult, ValueCondition valueCondition, ValidationContext validationContext);
}
