module Tests

open System
open Xunit

open MarsRoverLib.RoverLogic
open MarsRoverLib.RoverAgent
open Program



let getStates direction (currentX, currentY, destinationX, destinationY) =
    let currentState = {
        X = currentX
        Y = currentY
        direction = direction
    }
    
    let expectedState = {
        X = destinationX
        Y = destinationY
        direction = direction
    }
    (currentState, expectedState)

[<Literal>]
let gridWidth = 10

[<Literal>]
let gridHeight = 10

[<Literal>]
let maxY = gridHeight - 1

[<Literal>]
let maxX = gridWidth - 1

let boundaries = (gridWidth, gridHeight)

[<Theory>]
[<InlineData(0,0,0,maxY)>]
[<InlineData(5,5,5,4)>]
[<InlineData(0,4,0,3)>]
let ``Rover facing North moves forward`` testData =
    let current, expected = getStates North testData
    let destination = moveForward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,1)>]
[<InlineData(4,maxY,4,0)>]
let ``Rover facing South moves forward`` testData =
    let current, expected = getStates South testData
    let destination = moveForward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,1,0)>]
[<InlineData(maxX,0,0,0)>]
let ``Rover facing East moves forward`` testData =
    let current, expected = getStates East testData
    let destination = moveForward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,maxX,0)>]
[<InlineData(5,5,4,5)>]
let ``Rover facing West moves forward`` testData =
    let current, expected = getStates West testData
    let destination = moveForward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,1)>]
[<InlineData(0, maxY, 0, 0)>]
let ``Rover facing North moves backward`` testData =
    let current, expected = getStates North testData
    let destination = moveBackward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,maxY)>]
[<InlineData(4,4,4,3)>]
let ``Rover facing South moves backward`` testData =
    let current, expected = getStates South testData
    let destination = moveBackward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,maxX,0)>]
[<InlineData(5,5,4,5)>]
let ``Rover facing East moves backward`` testData =
    let current, expected = getStates East testData
    let destination = moveBackward boundaries current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(maxX,0,0,0)>]
[<InlineData(0,0,1,0)>]
let ``Rover facing West moves backward`` testData =
    let current, expected = getStates West testData
    let destination = moveBackward boundaries current
    Assert.Equal(expected, destination)

[<Fact>]
let ``Rover rotates right from North``() =
    let current = {
        X = 0
        Y = 0
        direction = North
    }
    let expected = {
        current with
            direction = East
    }
    let destination = rotateRight current
    Assert.Equal(expected, destination)

[<Fact>]
let ``Rover rotates right from West``() =
    let current = {
        X = 0
        Y = 0
        direction = West
    }
    let expected = {
        current with
            direction = North
    }
    let destination = rotateRight current
    Assert.Equal(expected, destination)

[<Fact>]
let ``Rover rotates left from North``() =
    let current = {
        X = 0
        Y = 0
        direction = North
    }
    let expected = {
        current with
            direction = West
    }
    let destination = rotateLeft current
    Assert.Equal(expected, destination)

[<Fact>]
let ``Rover rotates left from South``() =
    let current = {
        X = 0
        Y = 0
        direction = South
    }
    let expected = {
        current with
            direction = East
    }
    let destination = rotateLeft current
    Assert.Equal(expected, destination)

let startingState = {
    X = 4
    Y = 4
    direction = North
}

let marsMap : MarsMap = [
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; X; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
    [O; O; O; O; O; O; O; O; O; O;]
]

[<Fact>]
let ``Rover agent communicates state``() =
    let agent = MarsRoverLib.RoverAgent.roverAgent marsMap startingState
    let message, state = agent.PostAndReply(fun replyChannel -> GetState replyChannel)
    Assert.Equal(Ok "Rover ready", message)
    Assert.Equal(startingState, state)

[<Fact>]
let ``Rover agent detects ostacle``() =
    let agent = roverAgent marsMap startingState
    agent.Post MoveForward
    let message, state = agent.PostAndReply(fun replyChannel -> GetState replyChannel)
    Assert.Equal(Error "Encountered obstacle", message)
    Assert.Equal(startingState, state)

[<Fact>]
let ``Rover agent moves around``() =
    let agent = roverAgent marsMap startingState
    agent.Post RotateRight
    agent.Post MoveForward
    agent.Post MoveForward
    agent.Post MoveBackward
    agent.Post RotateLeft
    agent.Post RotateLeft
    let message, state = agent.PostAndReply(fun replyChannel -> GetState replyChannel)
    Assert.Equal(Ok "Rover rotated left", message)
    Assert.Equal({
        X = 5
        Y = 4
        direction = West
    }, state)

[<Fact>]
let ``parseMap correctly parses a string to a MarsMap``() =
    let str =
        """OOOOOOOOOO
OOOOOOOOOO
OOOOOOOOOO
OOOOXOOOOO
OOOOOOOOOO
OOOOOOOOOO
OOOOOOOOOO
OOOOOOOOOO
OOOOOOOOOO
OOOOOOOOOO"""
    let parsedMap = parseMap (str.Split(Environment.NewLine))
    let equality = (marsMap = parsedMap)
    Assert.True(equality)

[<Fact>]
let ``parseInstructions correctly parses a string to a sequence of Messages``() =
    let instructions = "ffbrlbf"
    let expected = [MoveForward; MoveForward; MoveBackward; RotateRight; RotateLeft; MoveBackward; MoveForward]
    let actual = parseInstructions instructions
    Assert.Equal(expected, actual)