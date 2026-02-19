# Assignment 3: Antymology 

## Name : Sahaj Malhotra
## UCID : 30144405

##  Project Overview & Motivation

This project extends the provided Antymology terrain simulation framework into a fully autonomous evolutionary system. The primary objective was to design, implement, and evaluate an adaptive agent capable of optimizing nest block production over successive generations within a procedurally generated voxel world.

The system introduces two agent types: worker ants and a single queen ant. While worker ants operate under baseline survival rules, the queen ant is controlled by a neural network whose parameters evolve over time. The evolutionary goal is simple yet non-trivial: maximize the number of nest blocks placed within a fixed evaluation window while respecting all environmental and mechanical constraints defined by the assignment.

The project integrates artificial intelligence, evolutionary computation, terrain interaction, and real-time simulation management into a stable and extensible Unity-based architecture. Evolutionary behaviour is observable across generations, as demonstrated by increasing nest production metrics in later generations compared to early random populations.

## System Architecture & Design Philosophy
The system is structured into modular components to maintain clarity, scalability, and maintainability.

Terrain Layer
The terrain system uses a chunk-based voxel structure. Each chunk dynamically generates mesh data based on underlying block states. Block types include:
Stone
Grass
Mulch
Acid
Container (boundary)
Nest (custom block)

Terrain updates trigger safe chunk regeneration, ensuring dynamic modifications (mulch consumption, digging, nest placement) are visually reflected without compromising simulation stability.

Agent Layer
The AntBase class implements shared survival and interaction mechanics:

Health decay per timestep
Acid-based accelerated decay
Mulch consumption
Height-constrained movement
Digging behaviour
Boundary-safe world interaction

The QueenAnt class inherits from AntBase and overrides behavioural decision-making using a neural network controller.

Evolution Layer
The EvolutionManager controls:

Population initialization
Candidate evaluation timing
Fitness measurement
Elite preservation
Mutation-based offspring generation
Generation cycling

This separation ensures terrain, agents, and evolution remain loosely coupled.

## Goal

The primary objective of this project is to design and implement an evolutionary algorithm that maximizes nest production within the simulated environment. The specific mechanisms governing reproduction, decision-making, and environmental interaction are entirely customizable, allowing full creative and technical freedom in the design of the agents.

Assessment focuses less on achieving optimal performance and more on the quality, clarity, and sophistication of the evolutionary system implemented. In other words, the emphasis is on demonstrating thoughtful algorithmic design, meaningful evolutionary dynamics, and observable adaptation over generations rather than producing perfectly optimized agents.

Therefore, the success of this project is measured by the presence of a well-structured evolutionary process and clear evidence of fitness-driven improvement, not by absolute nest production scores.

## Current Code
The project is organized into four primary components, all located within the Components folder:
Agents

Contains all agent-related logic, including the base ant class, the queen ant controller, the neural network implementation, and the evolutionary manager responsible for generation cycling and mutation.

Configuration
Stores configurable parameters that control world dimensions, chunk sizes, generation timing, mutation rates, and other simulation constants.
Terrain
Manages procedural world generation, block definitions, chunk mesh creation, and safe terrain updates when agents interact with the environment (e.g., consuming mulch, digging, or placing nest blocks).
UI
Handles user interface elements such as the live nest counter and simulation feedback.
To experience the simulation, simply open the project in Unity (version 6000.3 or compatible), load the SampleScene, and press Play. The environment will procedurally generate, ants will spawn automatically, and the evolutionary system will begin evaluating generations in real time.

### Agents
The Agents component contains the core behavioural and evolutionary logic of the system. This is where the majority of the project’s custom implementation resides.
This module is responsible for:

Ant movement and world interaction
Health management and decay logic
Mulch consumption and digging behaviour
Queen-specific nest production
Neural network decision-making
Evolutionary population management

The AntBase class defines shared survival mechanics, while the QueenAnt class extends this behaviour using a neural controller. The NeuralNetwork implementation enables adaptive decision-making, and the EvolutionManager coordinates generational cycling, evaluation, elite selection, and mutation.
This component ultimately governs how agents perceive their environment, make decisions, and evolve over time to maximize nest production.

### Configuration
The Configuration component centralizes simulation parameters and system-level settings.

The provided ConfigurationManager controls procedural terrain parameters such as:
World dimensions
Chunk diameter
World height
RNG seed

As additional behavioural and evolutionary parameters were introduced (e.g., evaluation duration, mutation rate, neural network size), they were structured in a way that allows easy modification and tuning without deeply altering core logic.

This separation ensures the system remains flexible and maintainable, enabling experimentation with different evolutionary dynamics without restructuring the architecture.

### Terrain
The Terrain component manages world memory, procedural generation, and visual representation.

The WorldManager is responsible for:

Initial terrain generation
Block storage and retrieval
Chunk mesh updates
Safe block modification during runtime

Block types include stone, grass, mulch, acid, container boundaries, and the custom NestBlock. Agents interact directly with this component when consuming mulch, digging, or placing nests.
Defensive boundary checks and chunk safety guards were added to ensure stability during dynamic world modification, allowing the evolutionary simulation to run across multiple generations without runtime errors.

### UI
The UI component handles user-facing visualization and simulation feedback.

Currently, this includes:

A fly camera for free exploration of the environment
A map editor for terrain inspection
A real-time nest counter displaying fitness during evaluation

The UI serves as a lightweight monitoring layer rather than a gameplay interface. Its primary purpose is to provide visual confirmation of nest production and evolutionary progress across generations.

## Requirements

### Admin Requirements
This project was developed in accordance with the assignment guidelines and submission policies outlined below.

The implementation was completed using Unity 2019 or later (Unity 6000.3 was used for development).

All development was maintained in a Git-based repository (GitHub) to ensure version control, traceability, and structured iteration.

The project originated from a fork of the provided base repository, preserving the original terrain framework.

Commits were made incrementally to reflect meaningful development milestones (neural network implementation, evolutionary loop integration, terrain safety fixes, UI addition, etc.). Frequent, descriptive commit messages were used to document design decisions and debugging progress.

All documentation is provided within this README.md, written in a portfolio format to clearly communicate the system design, architecture, and evolutionary methodology to an external reviewer or potential employer.

Screenshots and runtime evidence of generational progression are included to demonstrate observable evolutionary behaviour.


### Interface
The simulation provides a usable and transparent runtime interface for evaluation and grading.
Camera Controls

The camera remains fully usable during play mode. The grader can freely explore the scene using the provided fly camera system to inspect:

Ant movement
Queen behaviour
Terrain interaction
Nest placement
Evolutionary progression

No additional camera systems were required beyond the functional fly camera provided within the base framework.

UI Implementation
A real-time UI element displays:

Current number of nest blocks in the world
Fitness metric during each evaluation phase
This allows direct observation of performance and evolutionary improvement across generations.

### Ant Behaviour
All required behavioural constraints were implemented and enforced within the simulation.

Health System
Each ant maintains a measurable health value.
Health decreases by a fixed amount every timestep.
When health reaches zero, the ant is removed from the simulation.

Environmental Interaction
Ants may consume Mulch blocks to replenish health.
To consume mulch, an ant must be directly standing on the mulch block.
After consumption, the mulch block is removed from the world.
Mulch cannot be consumed if multiple ants occupy the same block.
Ants cannot move to a block with a height difference greater than 2 units.
Ants may dig blocks directly beneath them.
ContainerBlock types cannot be dug.
Standing on an AcidicBlock doubles the rate of health decay.

Resource Exchange
Ants may transfer health to other ants occupying the same space.
This transfer is zero-sum to preserve total system energy.

Queen Behaviour
Exactly one Queen ant exists during each evaluation phase.
The Queen is visually distinct from worker ants.
The Queen is responsible for producing nest blocks.
Producing a nest block costs one-third of the Queen’s maximum health.
Nest placement only occurs in valid air positions.
No new ants are created during evaluation (only between generations).

## Evolution Strategy
The evolutionary objective is to maximize nest production per evaluation cycle.

Development followed a staged approach:
Implement random baseline behaviour to ensure all mechanics function correctly.
Stabilize terrain modification and chunk updates.
Introduce a neural network to control the Queen’s decision-making.
Implement an evolutionary manager to evaluate, select, and mutate neural network weights.

Evolution Process
Each generation consists of a population of neural network candidates.
Each candidate is evaluated for a fixed duration.
Fitness = total nest blocks produced.
Top-performing networks are preserved (elite selection).
Remaining networks are generated via mutation of elites.
A new generation begins.

Over successive generations, nest production increases, demonstrating evolutionary adaptation.

## Submission
For submission:

The project was exported as a Unity package file.

The Unity package and supporting documentation were submitted via D2L under the designated Dropbox entry.

A link to the GitHub repository containing the full development history and commit logs was included in the submission message.

The repository demonstrates structured development, iterative debugging, and incremental feature implementation consistent with professional version control practices.
