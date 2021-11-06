import React from 'react';
import '../index.css';
import Login from './login.js';
import Lobby from './lobby.js';
import Game from './game.js';

export default class Navigation extends React.Component{
    constructor(props){
        super(props);
        this.state = {
          navigation: 0,
          ip: '',
          websocket:null,
          battleType:'',
        };
        this.connectToWebSockets = this.connectToWebSockets.bind(this);
        this.navigate = this.navigate.bind(this);
        this.chooseBattleType = this.chooseBattleType.bind(this);
    }

    navigate(page){
        if(page == 'login'){
            this.setState({navigation: 0});
        }
        else if(page == 'lobby'){
            this.setState({navigation: 1});
        }
        else if(page == 'game'){
            this.setState({navigation: 2});
        }
    }

    connectToWebSockets(ipAddress){
        this.setState({ip: ipAddress});
        this.navigate('lobby');
    }

    chooseBattleType(connection, battleType){
        this.setState({websocket: connection, navigation: 2, battleType: battleType});
    }


    render(){
        if(this.state.navigation == 1){
            return(<Lobby ip={this.state.ip} navigate={this.navigate} onSelection={this.chooseBattleType}/>);
        }
        else if(this.state.navigation == 2){
            if(this.state.websocket == null){
                alert('Disconnected from server, returning home.');
                this.navigate('login');
            }
            return(<Game 
                navigate={this.navigate}
                websocket={this.state.websocket}
                battleType={this.state.battleType}
                />);
        }
        else{
            return(<Login onConnect={this.connectToWebSockets}/>);
        }
    };
}