﻿using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.CliOutput
{


    public abstract class CliSection
    {
        protected CliSection(string header,
                             bool emitHeaderOnEmptyData = false)
        {
            Header = header;
            EmitHeaderWhenNoData = emitHeaderOnEmptyData;
        }

        public string Header { get; }
        public bool EmitHeaderWhenNoData { get; }

        public virtual IEnumerable<CliOutputUnit>? GetOpening(CliOutputContext context)
        => new CliOutputUnit[]
            {
                new CliHeading(Header ?? string.Empty)
            };

        public virtual IEnumerable<CliOutputUnit>? GetBody(CliOutputContext context) => null;

        public virtual IEnumerable<CliOutputUnit>? GetClosing(CliOutputContext context)
        => new CliOutputUnit[]
            {
                new CliText("")
            };
    }

    public abstract class CliSection<T> : CliSection
    {
        protected CliSection(CliFormatter formatter,
                      string header,
                      bool emitHeaderOnEmptyBody = false)
            : base(header, emitHeaderOnEmptyBody)
        {
            Data = new List<T>();
        }

        public IEnumerable<T> Data { get; }

    }
}

