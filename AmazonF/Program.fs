// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.Net
open System
open System.IO
open FSharp.Data

// Fetch the contents of a web page
let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) 
    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream)
    callback reader url

let myCallback (reader:IO.StreamReader) url = 
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

let GetLinkNameUrl (pUrl:string) = 
    let doc = HtmlDocument.Load(pUrl)
    let res = doc.CssSelect("ul.s-ref-indent-two > div > li > span > a") 
                |> Seq.choose (fun x -> x.TryGetAttribute "href"
                                             |> Option.map (fun a -> x.InnerText(), a.Value())
                              )
    res

[<EntryPoint>]
let main argv = 
    let url = "https://www.amazon.com/Televisions-Video/b/ref=sd_allcat_tv?ie=UTF8&node=1266092011"
    GetLinkNameUrl url
    // Create & configure HTTP web request
    

               
    //дet req = HttpWebRequest.Create(url) :?> HttpWebRequest 
    //let google = fetchUrl myCallback "https://www.amazon.com/Televisions-Video/b/ref=sd_allcat_tv?ie=UTF8&node=1266092011"
    printfn "%A" argv
    0 // return an integer exit code
