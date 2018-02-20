// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.Net
open System
open System.IO
open HtmlAgilityPack

module HtmlAgilityPack1 =

    open HtmlAgilityPack

    type HtmlNode with 
    
        member x.FollowingSibling name = 
            let sibling = x.NextSibling
            if sibling = null then
                null
            elif sibling.Name = name then
                sibling
            else 
                sibling.FollowingSibling name
    
        member x.FollowingSiblings name = seq {
            let sibling = x.NextSibling
            if sibling <> null then
                if sibling.Name = name then
                    yield sibling
                yield! sibling.FollowingSiblings name
        }

        member x.PrecedingSibling name = 
            let sibling = x.PreviousSibling
            if sibling = null then
                null
            elif sibling.Name = name then
                sibling
            else 
                sibling.PrecedingSibling name
    
        member x.PrecedingSiblings name = seq {
            let sibling = x.PreviousSibling
            if sibling <> null then
                if sibling.Name = name then
                    yield sibling
                yield! sibling.PrecedingSiblings name
        }

    let parent (node : HtmlNode) = 
        node.ParentNode

    let element name (node : HtmlNode) = 
        node.Element name

    let elements name (node : HtmlNode) = 
        node.Elements name

    let descendants (name:string) (node : HtmlNode) = 
        node.Descendants name

    let descendantsAndSelf name (node : HtmlNode) = 
        node.DescendantsAndSelf name

    let ancestors name (node : HtmlNode) = 
        node.Ancestors name

    let ancestorsAndSelf name (node : HtmlNode) = 
        node.AncestorsAndSelf name

    let followingSibling name (node : HtmlNode) = 
        node.FollowingSibling name

    let followingSiblings name (node : HtmlNode) = 
        node.FollowingSiblings name

    let precedingSibling name (node : HtmlNode) = 
        node.PrecedingSibling name

    let precedingSiblings name (node : HtmlNode) = 
        node.PrecedingSiblings name

    let inline innerText (node : HtmlNode) = 
        node.InnerText

    let inline attr name (node : HtmlNode) = 
        node.GetAttributeValue(name, "")

    let inline (?) (node : HtmlNode) name = 
        attr name node

    let inline hasAttr name value node = 
        attr name node = value

    let inline hasId value node = 
        hasAttr "id" value node

    let inline hasClass value node = 
        hasAttr "class" value node

    let inline hasText value (node : HtmlNode) = 
        node.InnerText = value

    let createDoc html =
        let doc = new HtmlDocument()
        doc.LoadHtml html
        doc.DocumentNode

// Fetch the contents of a web page
let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest 
    req.UserAgent <- "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream)
    callback reader url

let GetStreamUrl pReauest pUrlBuilder pUrlPart =
    let fullUrl:string = pUrlBuilder pUrlPart
    pReauest fullUrl 

let myCallback (reader:IO.StreamReader) url = 
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html

let rec GetLinkNameUrl pGetStreamUrl pUrlPart name listT = 
    let pUrlSteam:string = pGetStreamUrl pUrlPart
    let doc = HtmlAgilityPack1.createDoc(pUrlSteam)
    let uls = (fun (z:string) ->  doc.Descendants z)
                >> Seq.filter (fun x -> x.Attributes.Contains "class" )
                >> Seq.filter (fun x -> x.Attributes.["class"].Value.Contains("s-ref-indent-two") )
                >> Seq.map (fun x -> x.ChildNodes.[0].Descendants "li" )
                >> Seq.concat
                >> Seq.map( fun x -> x.SelectNodes (".//span/a")) 
                >> Seq.concat 
                >> Seq.map( fun x -> (x.Attributes.["href"].Value, x.InnerText))

    let resTuple = uls "ul"
    let recRes = resTuple |> Seq.map (fun c -> GetLinkNameUrl pGetStreamUrl (fst c) (snd c) listT)
    let resList = (name, pUrlPart, resTuple)::listT 
    let fd= ""
    let f1d= ""
    resList

let FullUrl l r = 
    sprintf "%s%s" l r
[<EntryPoint>]
let main argv = 
    //let google = fetchUrl myCallback "https://www.amazon.com/s/ref=lp_283155_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1&bbn=1000&ie=UTF8&qid=1518787887&rnid=1000"
    let baseUrl = "https://www.amazon.com"
    let url =  GetLinkNameUrl 
                (GetStreamUrl (fetchUrl myCallback) (FullUrl baseUrl))  
                "/s/ref=lp_1_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1%2Cn%3A173508&bbn=1&ie=UTF8&qid=1519043803&rnid=1"
                "123" 
                List.empty
    let f = ""
    let t = ""
    printfn "%A" argv
    0 // return an integer exit code
