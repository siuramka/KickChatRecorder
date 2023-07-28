### Websocket Chat Logger for Kick chatrooms

# KickChatRecorder

The WebSocket Chat Logger is a concurrent C# app designed to connect to multiple Pusher WebSocket servers, join chat rooms, and record the messages received to a Cassandra key-value database.
Leveraging the power of channels, this application efficiently handles incoming data streams and stores them in a database.
#

While creating this project I ran into a problem of multiple consumers/threads trying to open the same file and using a lot of resourses and time, around 80%. Basically I/O bound. 

# The solution
Using Cassandra key-value database and C# Channels

There are multiple Channels(Producers) to listen to that produce chat messages. These messages get consumed by multiple consumers concurrently from a thread safe FIFO ***Channels*** data structure that passes data asyncronisly. The messages are stored in a Cassandra key-value database, and is very effective by being able to write to database asyncronisly.

# Benchmarks 

When a producer connects to the test websocket server, he receives N messages, basically filling the channel. Then main thread waits for the test producer to receive the data and starts consuming.

10k messages per producer
Total: 200k

| Producers N  | Consumers N | Time (sec) |
| ------------- | ------------- | ------------- |
| 20	| 30 |	126 |
|20	| 20 |	114 |
|20	| 10 |	96 |
|20	| 5 |	174 |
|20|	1	| 471 |

2k messages per producer
Total: 20k

| Producers N  | Consumers N | Time (sec) |
| ------------- | ------------- | ------------- |
| 20	| 80 |	59 |
|20	| 30 |	14 |
|20	| 20 |	13 |
|20|	10	| 19 |
|20|	5	| 30 |
|20|	1	| 98 |

