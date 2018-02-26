// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.Net
open System
open System.IO
open System.Threading

let rec ReTry f t (s:int) :WebResponse =
    let multiply = System.Random().Next(2, 5)

    try
        Thread.Sleep s
        f()
    with
    | ex -> match t > 0 with
            | true  -> printfn "EXCEPTION: %s" (ex.ToString())
                       ReTry f (t-1) (s*multiply)
            | false -> printfn "FALSE: %s" (ex.ToString()) 
                       f()

let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest 
    req.UserAgent <- "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
    let wait = System.Random().Next(100, 500)
    use resp = ReTry req.GetResponse 5 wait
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream)
    callback reader url

let GetStreamUrl pReauest pUrlBuilder pUrlPart =
    let fullUrl:string = pUrlBuilder pUrlPart
    pReauest fullUrl 

let myCallback (reader:IO.StreamReader) url = 
    printfn "Downloaded %s." url 
    let html = reader.ReadToEnd()
    html

let rec GetLinkNameUrl pGetStreamUrl pUrlPart nameParent = 
    let pUrlSteam:string = pGetStreamUrl pUrlPart
    let doc = HtmlAgilityPackWraper.createDoc(pUrlSteam)
    let uls = (fun (z:string) ->  doc.Descendants z)
                >> Seq.filter (fun x -> x.Attributes.Contains "class" )
                >> Seq.filter (fun x -> x.Attributes.["class"].Value.Contains("s-ref-indent-two") )
                >> Seq.collect (fun x -> x.ChildNodes.[0].Descendants "li" )
                >> Seq.collect( fun x -> x.SelectNodes (".//span/a")) 
                >> Seq.map( fun x -> (nameParent |> string, x.Attributes.["href"].Value.Replace("&amp;", "&"), x.InnerText))

    let resTuple = uls "ul"
    //let recRes = resTuple |> Seq.map (fun c -> GetLinkNameUrl pGetStreamUrl (fst c) (snd c) listT)
    
    let f = ""
    let t = ""

    resTuple |> Seq.toList

let FullUrl l r = 
    sprintf "%s%s" l r
[<EntryPoint>]
let main argv = 
    //let google = fetchUrl myCallback "https://www.amazon.com/s/ref=lp_283155_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1&bbn=1000&ie=UTF8&qid=1518787887&rnid=1000"
    let baseUrl = "https://www.amazon.com"
    let GetLinkNameUrlCurry = GetLinkNameUrl (GetStreamUrl (fetchUrl myCallback) (FullUrl baseUrl))  
    let resTupleList = GetLinkNameUrlCurry
                        "/s/ref=lp_1_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1%2Cn%3A173508&bbn=1&ie=UTF8&qid=1519043803&rnid=1"
                        "Start" 
                
    let recFold = resTupleList |> Seq.fold (fun a (nameParent,url,urlName) -> (GetLinkNameUrlCurry url urlName)@a) resTupleList
    
    let f = ""
    let t = ""

    printfn "%A" argv
    0 // return an integer exit code
