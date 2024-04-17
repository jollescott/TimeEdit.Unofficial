namespace TimeEdit.Unofficial

open FSharp.Data;

module TimeEdit =
    let private BASE_URL = "https://cloud.timeedit.net/lu/web/lth1/";
    
    let queryProgram name =
        let queryUrl = $"%s{BASE_URL}objects.html?partajax=t&sid=1002&search_text=%s{name}&types=191"
        
        let queryPage = HtmlDocument.Load(queryUrl)
        let programObj = queryPage.Elements() |> Seq.filter (fun elem -> elem.AttributeValue("data-name") = name) |> Seq.head
        
        programObj.AttributeValue("data-idonly")
        
        