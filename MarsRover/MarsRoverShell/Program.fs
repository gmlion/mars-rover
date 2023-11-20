open System

open MarsRoverLib.RoverLogic
open MarsRoverLib.RoverAgent

let parseMap (lines: string array) : MarsMap =
    lines
    |> Seq.map
        (fun line ->
            line |> Seq.map (fun c -> if c = 'O' then O else X) |> List.ofSeq)
    |> List.ofSeq

let parseDirection input : Direction =
    if input = "N" then North
    else if input = "S" then South
    else if input = "W" then West
    else if input = "E" then East
    else failwith "Direction parsing error"

let parseInstructions input =
    let parseInstruction c =
        if c = 'f' then MoveForward
        else if c = 'b' then MoveBackward
        else if c = 'r' then RotateRight
        else if c = 'l' then RotateLeft
        else failwith "Instructions parsing error"

    input |> Seq.map parseInstruction

[<EntryPoint>]
let main _ =
    let mapFile = IO.File.ReadAllLines "map.txt"
    let marsMap = parseMap mapFile
    Console.WriteLine("Insert X coordinate")
    let x = Console.ReadLine() |> int
    Console.WriteLine("Insert Y coordinate")
    let y = Console.ReadLine() |> int
    Console.WriteLine("Insert direction (N,S,E,W)")
    let direction = parseDirection (Console.ReadLine())
    let startingState = {
        X = x
        Y = y
        direction = direction
    }

    let agent = MarsRoverLib.RoverAgent.roverAgent marsMap startingState
    Console.WriteLine("Insert instructions")
    let instructions = parseInstructions (Console.ReadLine()) |> Seq.toList
    let rec loop l =
        match l with
        | [] ->
            let message, _ = agent.PostAndReply(fun replyChannel -> GetState replyChannel)
            Console.WriteLine("All instructions executed")
        | instruction::rest ->
            agent.Post instruction
            let message, state = agent.PostAndReply(fun replyChannel -> GetState replyChannel)
            match message with
            | Error msg -> Console.WriteLine(msg)
            | Ok msg -> Console.WriteLine(msg); loop rest

    loop instructions
    // Return 0. This indicates success.
    0