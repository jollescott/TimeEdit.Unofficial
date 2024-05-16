#if INTERACTIVE
#r "nuget: FSharp.Data"
#r "nuget: Jint"
#else
namespace TimeEdit.Unofficial
#endif

open System.IO
open System.Reflection
open System.Text.RegularExpressions
open FSharp.Data
open Jint

module TimeEdit =
    let private BASE_URL = "https://cloud.timeedit.net/lu/web/lth1/";
    
    type public Document(window) =
        member public this.defaultView = window
       
    type public Window() =
        member public this.createElement(test: string) = printf $"%s{test}"
        member public this.document = Document(this)
        member public this.window = this
            
    let queryProgram programName =
        let queryUrl = $"%s{BASE_URL}objects.html?partajax=t&sid=1002&search_text=%s{programName}&types=191"
        let document = HtmlDocument.Load(queryUrl)
        document.Elements() |> Seq.filter (fun elem -> elem.AttributeValue("data-name") = programName) |> Seq.map (_.AttributeValue("data-idonly"))
        
    let getLinkbase () =
        let document = HtmlDocument.Load("https://cloud.timeedit.net/lu/web/lth1/ri1Q5006.html")
        document.Descendants() |> Seq.filter (_.HasId("links_base")) |> Seq.head |> (_.AttributeValue("data-base"))
       
    let documentStubJs () =
        let assembly = Assembly.GetExecutingAssembly()
        
        #if INTERACTIVE
        use reader = new StreamReader("./TimeEdit.Unofficial/stub.js")
        #else
        use stream = assembly.GetManifestResourceStream("stub.js")
        use reader = new StreamReader(stream)
        #endif
        reader.ReadToEnd()
        
    let interpretScrambler () =
        let document = HtmlDocument.Load("https://cloud.timeedit.net/lu/web/lth1/ri1Q5006.html")
        let initJs = document.Descendants["script"] |> Seq.map (_.InnerText()) |> Seq.head
        let mainJsPath = Regex.Match(initJs, "'\/static\/.+?\/min.js'").Value.Replace("'", "")
        let mainJsUrl = $"https://cloud.timeedit.net%s{mainJsPath}"
        
        let mainJs = Http.RequestString(mainJsUrl)
                
        let window = Window()
        
        let wrapperCodeJs = $"function wrapper() {{ %s{mainJs} }};"
        
        let engine = new Engine() |> _.SetValue("window", window) |> _.Execute(wrapperCodeJs) |> _.Execute("wrapper.call(window);")
        engine.Dispose()