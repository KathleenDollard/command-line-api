﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Extended.Annotations;

namespace System.CommandLine.Extended;

/// <summary>
/// Extensions for <see cref="CliSymbol"/> that allow fluent construction of <see cref="CliCommand"/> and fluent addition of annotations to <see cref="CliSymbol"/> objects.
/// </summary>
static class FluentSymbolExtensions
{
    public static TSymbol With<TSymbol, TValue>(this TSymbol symbol, AnnotationAccessor<TValue> annotation, TValue value)
        where TSymbol : CliSymbol
    {
        annotation.Set(symbol, value);
        return symbol;
    }

    public static TCommand With<TCommand>(this TCommand command, CliCommand subcommand)
        where TCommand : CliCommand
    {
        command.Add(subcommand);
        return command;
    }

    public static TCommand With<TCommand>(this TCommand command, CliOption option)
        where TCommand : CliCommand
    {
        command.Add(option);
        return command;
    }

    public static TCommand With<TCommand>(this TCommand command, CliArgument argument)
        where TCommand : CliCommand
    {
        command.Add(argument);
        return command;
    }

    public static TCommand With<TCommand>(this TCommand command, params CliSymbol[] symbols)
        where TCommand : CliCommand
    {
        foreach (var symbol in symbols)
        {
            command.Add(symbol);
        }
        return command;
    }
}
