## Principles (of this design)

* Anything we can do, they can do better - well at least they should be able to do
  * An alternate help system is needed to be sure ours can be extended. It could not.
* Context is for passing between layers and should have no internal content

## Responsibilities

Configuration is for permanent things and factories for current run things
Context is for current run

Cli versions are generic, Help versions are specific


### CliOutputConfiguration
Sections prop: List<CliSection> --> This simplifies the common case, and a customer renderer can modify based on context
GetFormatter(OutputContext) --> default to console

### CliHelpConfiguration

GetSymbolInspector(HelpContext)
*Defaults --> consider removing

*Try to collapse DefaultHelpConfiguration and HelpConfiguration (in help dll)*

### CliOutputContext

OutputConfiguration (for sections and formatter)
MaxWidth
Writer
~~Formatter~~ - Custom formatters must be done via the configuration override or in the OutputRenderer

### HelpContext

ParseResult
CliConfiguration
Command -> It is not clear when this would be used other than testing - where it is rather difficult to
           create a parseResult without excess scope sharing. And it is also a convenience.

### CliOutputRender

protected virtual
* GetSections(outputContext) --> defaults to the one in outputContext.OutputConfiguration
* GetFormatter(outputContext) --> defaults to the method in outputContext.OutputConfiguration













Inspector retrieves data from Symbols

* This could be located in the Parsing assembly because it may be useful in 
* May be a static class
 
The structure that is built is:

* Sections which contain blocks
  * Blocks are either tables or text
  * Text
    * Lambda expression that returns the text to display
    * Parameters to lambda are an inspector and a "formatter" (easily misunderstood term)
      * Formatter supports various output - HTML, markdown, etc.following a base class format
  * Table
    * Headers - which may be null - lambdas to allow formatting
    * Lambdas for column entries - this is the same as for text

There is no opening or closing as these would just be more sections

Issues:
* Who has resposiblity for formatting default values (like the separator for enumberables)?
  * GetDefaultValue is also available and we will have a helper for standard output
  * Inspector - let's get a default from the start
  * Configuration - let's have common place to set things
  * The lambda where it is used - combine all customizations 
* Similar question for usage
* Similar alias concatenation

Current answer:

Configuration will have a series of "Fallback...Text" methods.
* `Falback` chose over `Default` because it avoids `DefaultDefaultValueText`
* `Falback` chose over `Standard` as that has judgement
* `Fallback` is a poor choice because there is no actual fallback mechanism


