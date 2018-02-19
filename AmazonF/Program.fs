// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.Net
open System
open System.IO
open FSharp.Data

// Fetch the contents of a web page
let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest 
    req.UserAgent <- "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream)
    callback reader url

let fetchUrlStream url =        
    let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest 
    req.UserAgent <- "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    stream

let GetStreamUrl pReauest pUrlBuilder pUrlPart =
    let fullUrl:string = pUrlBuilder pUrlPart
    pReauest fullUrl
    

let myCallback (reader:IO.StreamReader) url = 
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

let rec GetLinkNameUrl pGetStreamUrl pUrlPart name listT = 
    let pUrlSteam = pGetStreamUrl pUrlPart
    let doc = HtmlDocument.Parse(pUrlSteam)
    let fds = (doc.Descendants "ul")
    let fdsf = (doc.Descendants "ul").CssSelect(".s-ref-indent-two")
    let dsa = doc.CssSelect("ul.a-unordered-list > .s-ref-indent-two") 
    let dsa1 = doc.CssSelect("div.s-padding-right-mini s-border-right > ") 
    let res = doc.CssSelect("ul.s-ref-indent-two > div > li > span > a") 
                |> Seq.choose (fun x -> x.TryGetAttribute "href"
                                             |> Option.map (fun a -> x.InnerText(), a.Value())
                              ) 
    //let hhhh = res |> Seq.map (fun c -> GetLinkNameUrl pGetStreamUrl (fst c) (snd c) listT)
    //let hyh = (name, pUrl, res)::listT 
    let fd= ""
    let f1d= ""
    res
let FullUrl l r = 
    sprintf "%s%s" l r
[<EntryPoint>]
let main argv = 
    let google = fetchUrl myCallback "https://www.amazon.com/s/ref=lp_283155_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1&bbn=1000&ie=UTF8&qid=1518787887&rnid=1000"
    let baseUrl = "https://www.amazon.com"
    let url =  GetLinkNameUrl 
                (GetStreamUrl (fetchUrl myCallback) (FullUrl baseUrl))  
                "/s/ref=lp_1_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1%2Cn%3A173508&bbn=1&ie=UTF8&qid=1519043803&rnid=1"
                "123" 
                List.empty
    //let gg =  url |> Seq.toList
    //let hh = fst gg.Head
   // let hh1 = snd gg.Head
    //let gg1 = GetLinkNameUrl 

    // Create & configure HTTP web request
    

               
    //дet req = HttpWebRequest.Create(url) :?> HttpWebRequest 
    //let google = fetchUrl myCallback "https://www.amazon.com/Televisions-Video/b/ref=sd_allcat_tv?ie=UTF8&node=1266092011"
    printfn "%A" argv
    0 // return an integer exit code
