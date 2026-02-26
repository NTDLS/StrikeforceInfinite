# Concepts

The server should accept lobby hosting requests for multiple game clients.
Each client can start their own lobby on this server, and clients can join those lobbies.
The server handles routing messages between clients in the same lobby.

# Implementation

The server is implemented using a combination of reliable and datagram messaging protocols.

Reliable Messaging is used for propping up lobbies, joining/leaving lobbies, and other critical operations that require guaranteed delivery.
Datagram Messaging is used for game communication, such as chat messages, position updates, etc. where low latency is more important than guaranteed delivery.

Each player will be responsible for sending their own position updates and other game-related messages to the server,
which will then route those messages to other players in the same lobby. The server act as a central hub for all
communication between clients, ensuring that messages are delivered to the correct recipients based on lobby membership
as well as function as a headless game engine that can run game logic for AI players and other server-side game mechanics.

