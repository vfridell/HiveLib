using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;
using HiveLib.Models;
using HiveLib.AI;
using HiveLib.ViewModels;

namespace SlackBot
{
    public class HiveGameMiddleware : MiddlewareBase
    {
        private readonly HiveGamePlugin _hiveGamePlugin;

        public HiveGameMiddleware(IMiddleware next, HiveGamePlugin hiveGamePlugin) : base(next)
        {
            _hiveGamePlugin = hiveGamePlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"start"},
                    Description = "Start a new game of Hive",
                    EvaluatorFunc = StartGameHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{"end"},
                    Description = "Quit the current game of Hive",
                    EvaluatorFunc = QuitGameHandler
                },
                new HandlerMapping
                {
                    ValidHandles = new []{"move", ""},
                    Description = "Make a move against the AI",
                    EvaluatorFunc = OtherPlayerMoveHandler
                }

            };
        }

        private IEnumerable<ResponseMessage> OtherPlayerMoveHandler(IncomingMessage message, string matchedHandle)
        {
            if (message.BotIsMentioned)
            {
                string notation = message.TargetedText.Substring(matchedHandle.Length).Trim();

                Move move;
                if (!Move.TryGetMove(notation, out move))
                {
                    yield return message.ReplyToChannel($"invalid move notation: {notation}");
                }
                else if (!_hiveGamePlugin.TryMakeOtherPlayerMove(move))
                {
                    yield return message.ReplyToChannel($"invalid move: {_hiveGamePlugin.GetLastGameError()}");
                }
                else
                {
                    Move aiMove;
                    if (!_hiveGamePlugin.TryMakeAIMove(out aiMove))
                    {
                        yield return message.ReplyToChannel($"Error with AI move: {_hiveGamePlugin.GetLastGameError()}");
                    }
                    else
                    {
                        yield return message.ReplyToChannel($"{_hiveGamePlugin.GetUser()}: {aiMove.notation}");
                    }
                }
            }
        }

        private IEnumerable<ResponseMessage> QuitGameHandler(IncomingMessage message, string matchedHandle)
        {
            if (message.BotIsMentioned)
            {
                yield return message.ReplyToChannel("Quitting game...");
                _hiveGamePlugin.QuitGame();
            }
        }

        private IEnumerable<ResponseMessage> StartGameHandler(IncomingMessage message, string matchedHandle)
        {
            if (message.BotIsMentioned)
            {
                string parameters = message.TargetedText.Substring(matchedHandle.Length).Trim();
                var paramArray = parameters.Split(' ');
                string user;
                string color;
                if (paramArray.Length == 2 || paramArray.Length == 1)
                {
                    if(paramArray.Length == 1)
                    {
                        user = message.Username;
                        color = paramArray[0];
                    }
                    else
                    {
                        user = paramArray[0];
                        color = paramArray[1];
                    }

                    _hiveGamePlugin.SaveUsername(user);
                    if (!_hiveGamePlugin.GameStarted())
                    {
                        bool playingWhite = false;
                        if (color.ToLower() == "w" || color.ToLower() == "white")
                        {
                            playingWhite = true;
                            yield return message.ReplyToChannel($"{user}: start b");
                        }
                        else
                        {
                            playingWhite = false;
                            yield return message.ReplyToChannel($"{user}: start w");
                        }

                        _hiveGamePlugin.BeginNewGame(playingWhite, user);
                        if (playingWhite)
                        {
                            Move aiMove;
                            if (!_hiveGamePlugin.TryMakeAIMove(out aiMove))
                            {
                                yield return message.ReplyToChannel($"Error with AI move: {_hiveGamePlugin.GetLastGameError()}");
                            }
                            else
                            {
                                yield return message.ReplyToChannel($"{_hiveGamePlugin.GetUser()}: {aiMove.notation}");
                            }
                        }

                    }
                    else
                    {
                        yield return message.ReplyToChannel($"Game already started with {_hiveGamePlugin.GetUser()}");
                    }
                }
                else
                {
                    yield return message.ReplyToChannel($"you must give a user and a color:  start <username> <color>");
                }
            }
        }


    }
}