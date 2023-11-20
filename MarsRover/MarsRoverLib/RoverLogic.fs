module MarsRoverLib.RoverLogic

open Library

(*
The rover is in a grid represented by X and Y coordinates, where (0,0) is the top left corner.
X represents a Cell with an ostacle on it, while O represent a Cell with no obstacle.
*)

type Cell = O | X
type MarsMap = (Cell List) List



(*
The Rover state is described by its X and Y coordinates, and the direction it is currently facing.
*)

type Direction = North | East | South | West

type Move = Forward = 1 | Backward = -1

type RoverState = {
    X : int
    Y : int
    direction : Direction
}

(*
This function returns the new Rover state depending on the type of movement (Forward, Backward) and the current Rover state.
The order of the arguments will aid in the creation of curried functions next.
Delta is the numerical representation of the enum describing the type of movement and is +1 for Forward and -1 for Backward.
*)

let moveRover (move: Move) (gridWidth, gridHeight) currentState  =
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
"Helper" functions with the two possible types of movement, obtained using partial application on the moveRover function.
*)
let moveForward = moveRover Move.Forward
let moveBackward = moveRover Move.Backward

(*
Rotation is defined by an enum. Its two values are used to increment or decrement an indexer to a circular sequence of the four possible directions.
*)

type RotatingDirection = Left=(-1) | Right=1

let rotatedDirection (turningDirection: RotatingDirection) startingDirection =
    let delta = LanguagePrimitives.EnumToValue turningDirection
    let directionSequence = [North; East; South; West]
    let currentIndex = directionSequence |> Seq.findIndex (fun el -> el = startingDirection)
    let destinationIndex = (currentIndex + delta) %! directionSequence.Length
    directionSequence.[destinationIndex]

let rotate td currentState =
    {
        currentState with
            direction = rotatedDirection td currentState.direction
    }

let rotateRight = rotate RotatingDirection.Right
let rotateLeft = rotate RotatingDirection.Left

(*
Rover has to detect an obstacle before moving.
*)

let isObstacle (marsMap: MarsMap) (x, y) =
    match marsMap.[y].[x] with
        | O -> false
        | X -> true