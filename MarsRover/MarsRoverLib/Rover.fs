module MarsRoverLib.Rover

open Library

(*
The rover is in a grid represented by X and Y coordinates, where (0,0) is the top left corner.

We define the grid size as literal (static constant) values.
*)

[<Literal>]
let gridWidth = 10

[<Literal>]
let gridHeight = 10


type Direction = North | West | South | East

type Move = Forward = 1 | Backward = -1

(*
The Rover state is described by its X and Y coordinates, and the direction it is currently facing.
*)
type RoverState = {
    X : int
    Y : int
    direction : Direction
}

(*
This function returns the new Rover state depending on the type of movement (Forward, Backward) and the current Rover state.
The order of the arguments will aid in the creation of curried functions next.
*)

let moveRover (move: Move) currentState  =
    let delta = LanguagePrimitives.EnumToValue move

    match currentState.direction with
    | North ->
        {
            currentState with
                Y = (currentState.Y - delta) %! gridHeight
        }
    | West ->
        {
            currentState with
                X = (currentState.X - delta) %! gridWidth
        }
    | South ->
        {
            currentState with
                Y = (currentState.Y + delta) %! gridHeight
        }
    | East ->
        {
            currentState with
                X = (currentState.X + delta) %! gridWidth
        }

(*
"Helper" functions with the two possible types of movement
*)
let moveForward = moveRover Move.Forward
let moveBackward = moveRover Move.Backward
    

(*
type TurningDirection = Left=(-1) | Right=1

let turn (turningDirection: TurningDirection) startingDirection =
    let directionSequence = [North, West, South, East]

*)