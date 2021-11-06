import React from 'react';
import '../index.css';
import Board from './board.js';
import { models } from '../networkEvents/models.js';

export default class Game extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            squares: Array(9).fill(null),
            playerTeam: 0,
            turn: 0,
            winner:0,
            complete:false,
            isConnected: false,
            playerId: '',
            gameId: 0,
            timer: 0,
        };
        this.setSocket();
    }
    
    setSocket(){
      
      var createGameModel = models.createGameModel;
      if(this.props.battleType == 'ai'){
        createGameModel.IsAi = true;
      }else{
        createGameModel.IsAi = false;
      }
      this.send(createGameModel);

      this.props.websocket.onmessage = evt => {
          var json = JSON.parse(evt.data);
          if(json.ModelType == 2){
            this.setState({
              squares: Array(9).fill(null),
              turn: json.Turn,
              playerTeam: json.PlayerTeam,
              isConnected: true,
              playerId: json.PlayerId,
              gameId: json.GameId,
              timer: 30,
            });
            setInterval(this.timer, 1000);
          }
          if(json.ModelType == 4){
            var squares = Array(9).fill(null);
            for(const [i, square] of json.Squares.entries()){
              if(square == "X" || square == "O"){
                squares[i] = square;
              }
            }
            this.setState({
              squares: squares,
              turn: json.Turn,
              complete: json.Complete,
              winner: json.Winner,
              timer: 30,
            })
          }
      }
      
      this.props.websocket.onclose = () => {
        console.log('disconnected')
        alert('Server Disconnected, returning home.');
        this.props.navigate('login');
      }
    }

    timer = () =>{
      if(this.state.timer && this.state.timer > 0){
        this.setState({timer: this.state.timer -1});
      }
    }

    send(model){
      this.props.websocket.send(JSON.stringify(model));
    }

    handleClick(i){
      if(this.state.turn != this.state.playerTeam){
        return;
      }
      if(this.state.complete){
        return;
      }
      const squares = this.state.squares.slice();
        if(squares[i]){
            return;
        }
        var model = models.placeMarkerModel;
        model.PlayerId = this.state.playerId;
        model.GameId = this.state.gameId;
        model.Place = i;
        this.send(model);
      }
      

  render(){
    if(!this.state.isConnected){
      return(
        <div><h2>Connecting...</h2></div>
    );
    }else{
      return this.renderGame();
    }
  }    
  renderGame() {
      var squares = this.state.squares;
      let winner;
      if(this.state.complete){
        winner = true;
      }
      let status;
      let btn;
      let timer;
      if(winner){
        if(this.state.winner == -1){
          status = 'Draw!';
        }else if(this.state.winner == this.state.playerTeam){
          status = 'You Win!';
        }else{
          status = 'You Lose!';
        }
        btn = <div><button onClick={()=>this.props.navigate('lobby')}>Go Back</button></div>
      } else{
          if(this.state.turn == this.state.playerTeam){
            status = 'Your Turn';
          }else{
            status = 'Enemy Turn';
          }
          timer = <div>{this.state.timer}s Remaining</div>
      }

    return (
      <div className="game">
        <div className="game-board">
          <Board 
              squares={squares}
              onClick={(i) => this.handleClick(i)}
          />
        </div>
        <div className="game-info">
          <div>Your team is: {this.state.playerTeam == 0 ? 'X' : 'O'}</div>
          <div>{status}</div>
          {timer}
          {btn}
        </div>
      </div>
    );
  }
}