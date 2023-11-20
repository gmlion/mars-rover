module MarsRoverLib.RoverAgent

(*
The domain maps nicely with an "actor-based" implementation.
F# natively support this approach with MailboxProcessor.
For simplicity we're using a single agent representing the Rover, receiving commands as Messages.
*)

type Message =
    | GetState of AsyncReplyChannel<Result<string,string> * RoverLogic.RoverState>
    | MoveForward
    | MoveBackward
    | RotateLeft
    | RotateRight

let roverAgent (marsMap: RoverLogic.MarsMap) startingState =
    let boundaries = (marsMap.[0].Length, marsMap.Length)
    let moveForward = RoverLogic.moveForward boundaries
    let moveBackward = RoverLogic.moveBackward boundaries
    MailboxProcessor.Start(fun inbox -> 
        let rec loop (oldMessage, oldState) =
            async {
                let! msg = inbox.Receive()
                match msg with
                | GetState replyChannel ->
                    replyChannel.Reply(oldMessage, oldState)
                    return! loop (oldMessage, oldState)
                | MoveForward ->
                    let newState = moveForward oldState
                    if RoverLogic.isObstacle marsMap (newState.X, newState.Y) then
                        let newMessage = Error "Encountered obstacle"
                        return! loop (newMessage, oldState)
                    else
                        let newMessage = Ok "Rover moved forward"
                        return! loop (newMessage, newState)
                | MoveBackward ->
                    let newState = moveBackward oldState
                    if RoverLogic.isObstacle marsMap (newState.X, newState.Y) then
                        let newMessage = Error "Encountered obstacle"
                        return! loop (newMessage, oldState)
                    else
                        let newMessage = Ok "Rover moved backward"
                        return! loop (newMessage, newState)
                | RotateLeft ->
                    let newState = RoverLogic.rotateLeft oldState
                    let newMessage = Ok "Rover rotated left"
                    return! loop (newMessage, newState)
                | RotateRight ->
                    let newState = RoverLogic.rotateRight oldState
                    let newMessage = Ok "Rover rotated right"
                    return! loop (newMessage, newState)
            }
        loop (Ok "Rover ready", startingState))