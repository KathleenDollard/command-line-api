﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using System.CommandLine.Parsing;
using System.CommandLine.ValueConditions;
using System.CommandLine.ValueSources;
using Xunit;

namespace System.CommandLine.Subsystems.Tests;

public class ValueSourceTests
{
    private PipelineResult EmptyPipelineResult(string input = "", params CliValueSymbol[] valueSymbols)
    {
        var rootCommand = new CliRootCommand();
        foreach (var symbol in valueSymbols)
        {
            rootCommand.Add(symbol);
        }
        var parseResult = CliParser.Parse(rootCommand, input);
        return new PipelineResult(parseResult, "", Pipeline.CreateEmpty());
    }

    [Fact]
    public void SimpleValueSource_with_set_value_retrieved()
    {
        var valueSource = new SimpleValueSource<int>(42);

        (bool success, int value) = valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
            .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void SimpleValueSource_with_converted_value_retrieved()
    {
        ValueSource<int> valueSource = 42;

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void SimpleValueSource_created_via_extension_value_retrieved()
    {
        var valueSource = ValueSource.Create(42);

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
            .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void CalculatedValueSource_produces_value()
    {
        var valueSource = new CalculatedValueSource<int>(() => (true, 42));

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
            .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void CalculatedValueSource_implicitly_converted_produces_value()
    {
        // TODO: Figure out why this doesn't work, and remove implicit operator if it does not work
        // ValueSource<int> valueSource2 = (() => 42);
        ValueSource<int> valueSource = (ValueSource<int>)(() => (true, 42)); ;

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void CalculatedValueSource_from_extension_produces_value()
    {
        var valueSource = ValueSource.Create(() => (true, 42));
        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult());

        success.Should()
            .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void RelativeToSymbolValueSource_produces_value_that_was_set()
    {
        var option = new CliOption<int>("-a");
        var valueSource = new RelativeToSymbolValueSource<int>(option);

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult("-a 42", option));

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void RelativeToSymbolValueSource_implicitly_converted_produces_value_that_was_set()
    {
        var option = new CliOption<int>("-a");
        ValueSource<int> valueSource = option;

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult("-a 42", option));

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void RelativeToSymbolValueSource_from_extension_produces_value_that_was_set()
    {
        var option = new CliOption<int>("-a");
        var valueSource = new RelativeToSymbolValueSource<int>(option);

        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult("-a 42", option));

        success.Should()
            .BeTrue();
        value.Should()
            .Be(42);
    }

    [Fact]
    public void RelativeToEnvironmentVariableValueSource_produces_value_that_was_set()
    {
        var envName = "SYSTEM_COMMANDLINE_TESTING";
        var valueSource = new RelativeToEnvironmentVariableValueSource<int>(envName);
        
        Environment.SetEnvironmentVariable(envName, "42");
        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult(""));
        Environment.SetEnvironmentVariable(envName, null);

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }
 

    [Fact]
    public void RelativeToEnvironmentVariableValueSource_from_extension_produces_value_that_was_set()
    {
        var envName = "SYSTEM_COMMANDLINE_TESTING";
        var valueSource = ValueSource.CreateFromEnvironmentVariable<int>(envName);

        Environment.SetEnvironmentVariable(envName, "42");
        (bool success, int value) =  valueSource.GetTypedValue(EmptyPipelineResult(""));
        Environment.SetEnvironmentVariable(envName, null);

        success.Should()
           .BeTrue();
        value.Should()
            .Be(42);
    }
}
