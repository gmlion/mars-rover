# Mars Rover

You’re part of the team that explores Mars by sending remotely controlled vehicles to the surface of the planet. Develop an API that translates the commands sent from earth to instructions that are understood by the rover.

Requirements

            • You are given the initial starting point (x,y) of a rover and the direction (N,S,E,W) it is facing.
            • The rover receives a character array of commands.
            • Implement commands that move the rover forward/backward (f,b).
            • Implement commands that turn the rover left/right (l,r).
            • Implement wrapping from one edge of the grid to another. (planets are spheres after all)
            • Implement obstacle detection before each move to a new square. If a given sequence of commands encounters an obstacle, the rover moves up to the last possible point, aborts the sequence and reports the obstacle.

## Notes about implementation:
The logic is coded in RoverLogic.fs using pure functions.
Even if it's not stricly needed given the requirements, the Rover itself is modeled as an agent using an "actor-based" approach since the domain maps quite nicely to this representation.
Asyncronous agents in F# can be created using MailboxProcessor; in RoverAgent.fs the MailboxProcessor receives Messages and uses the pure functions previously defined in RoverLogic to "keep" recursively a current message and its position.
