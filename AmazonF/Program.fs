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

let rec GetLinkNameUrl pUrlBuilder pUrl name listT = 
    let fullUrl:string = pUrlBuilder pUrl
    let doc = HtmlDocument.Load(fullUrl)
    let res = doc.CssSelect("ul.s-ref-indent-two > div > li > span > a") 
                |> Seq.choose (fun x -> x.TryGetAttribute "href"
                                             |> Option.map (fun a -> x.InnerText(), a.Value())
                              ) 
    let hhhh = res |> Seq.map (fun c -> GetLinkNameUrl pUrlBuilder (fst c) (snd c) listT)
    let hyh = (name, pUrl, res)::listT 
    
    res
let FullUrl l r = 
    sprintf "%s%s" l r
[<EntryPoint>]
let main argv = 
    
    let baseUrl = "https://www.amazon.com"
    let url =  GetLinkNameUrl (FullUrl baseUrl) "/s/ref=lp_283155_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1&bbn=1000&ie=UTF8&qid=1518787887&rnid=1000" "123" List.empty
    let gg =  url |> Seq.toList
    let hh = fst gg.Head
    let hh1 = snd gg.Head
    //let gg1 = GetLinkNameUrl 

    // Create & configure HTTP web request
    

               
    //дet req = HttpWebRequest.Create(url) :?> HttpWebRequest 
    //let google = fetchUrl myCallback "https://www.amazon.com/Televisions-Video/b/ref=sd_allcat_tv?ie=UTF8&node=1266092011"
    printfn "%A" argv
    0 // return an integer exit code
