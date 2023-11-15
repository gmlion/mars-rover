module Tests

open System
open Xunit

open MarsRoverLib.Rover

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
let maxY = MarsRoverLib.Rover.gridHeight - 1

[<Literal>]
let maxX = MarsRoverLib.Rover.gridWidth - 1

[<Theory>]
[<InlineData(0,0,0,maxY)>]
[<InlineData(5,5,5,4)>]
[<InlineData(0,4,0,3)>]
let ``Rover facing North moves forward`` testData =
    let current, expected = getStates North testData
    let destination = moveForward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,1)>]
[<InlineData(4,maxY,4,0)>]
let ``Rover facing South moves forward`` testData =
    let current, expected = getStates South testData
    let destination = moveForward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,1,0)>]
[<InlineData(maxX,0,0,0)>]
let ``Rover facing East moves forward`` testData =
    let current, expected = getStates East testData
    let destination = moveForward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,maxX,0)>]
[<InlineData(5,5,4,5)>]
let ``Rover facing West moves forward`` testData =
    let current, expected = getStates West testData
    let destination = moveForward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,1)>]
[<InlineData(0, maxY, 0, 0)>]
let ``Rover facing North moves backward`` testData =
    let current, expected = getStates North testData
    let destination = moveBackward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,0,maxY)>]
[<InlineData(4,4,4,3)>]
let ``Rover facing South moves backward`` testData =
    let current, expected = getStates South testData
    let destination = moveBackward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(0,0,maxX,0)>]
[<InlineData(5,5,4,5)>]
let ``Rover facing East moves backward`` testData =
    let current, expected = getStates East testData
    let destination = moveBackward current
    Assert.Equal(expected, destination)

[<Theory>]
[<InlineData(maxX,0,0,0)>]
[<InlineData(0,0,1,0)>]
let ``Rover facing West moves backward`` testData =
    let current, expected = getStates West testData
    let destination = moveBackward current
    Assert.Equal(expected, destination)