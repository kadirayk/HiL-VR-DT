(define (problem dobot01)
(:domain BLOCKS)
(:objects BlueCube RedCube1 RedCube2 YellowCube - block
posred posblue posyellow - position)
(:init (HANDEMPTY) (free posred)(free posblue)(free posyellow)(ontable RedCube2)(ontable BlueCube)(ontable RedCube1)(on YellowCube BlueCube)(clear RedCube2)(clear YellowCube)(clear RedCube1))
(:goal (and (at YellowCube posyellow)(at BlueCube posblue)(at RedCube1 posred)(on RedCube2 RedCube1)))
)