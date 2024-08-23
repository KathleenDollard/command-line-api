﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.ValueSources;

public class AggregateValueSource<T> : ValueSource<T>
{
    private List<ValueSource<T>> valueSources = [];

    public AggregateValueSource(ValueSource<T> firstSource,
                                 ValueSource<T> secondSource,
                                 string? description = null,
                                 params ValueSource<T>[] otherSources)
    {
        valueSources.AddRange([firstSource, secondSource, .. otherSources]);
        Description = description;
    }


    public override string? Description { get; }

    public bool PrecedenceAsEntered { get; set; }

    public override (bool success, T? value) GetTypedValue(PipelineResult pipelineResult) 
        => ValueFromSources(pipelineResult);

    private (bool success, T? value) ValueFromSources(PipelineResult pipelineResult)
    {
        var orderedSources = PrecedenceAsEntered
            ? valueSources
            : [.. valueSources.OrderBy(GetPrecedence)];
        foreach (var source in orderedSources)
        {
            (var success, var value) = source.GetTypedValue(pipelineResult);
            if (success)
            {
                return (true, value);
            }
        }
        return (false, default);
    }

    internal static int GetPrecedence(ValueSource<T> source)
    {
        return source switch
        {
            SimpleValueSource<T> => 0,
            CalculatedValueSource<T> => 1,
            RelativeToSymbolValueSource<T> => 2,
            //RelativeToConfigurationValueSource<T> => 3,
            RelativeToEnvironmentVariableValueSource<T> => 4,
            _ => 5
        };
    }
}


