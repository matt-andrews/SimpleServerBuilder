import React from 'react';
import '../index.css';
import { models } from '../networkEvents/models.js';

export default class MainScene extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            isConnected: false,
        }
    }
    
    ws = new WebSocket(this.props.ip);

    componentDidMount() {
        this.ws.onopen = () => {
        // on connecting, do nothing but log it to the console
        console.log('connected')
        var model = models.connectionStateModel;
        this.send(JSON.stringify(model));
        this.setState({isConnected:true});
        }

        this.ws.onmessage = evt => {
        // listen to data sent from the websocket server
        const message = JSON.parse(evt.data)
        this.setState({dataFromServer: message})
        console.log(message)
        }

        this.ws.onclose = () => {
        console.log('disconnected')
        alert('Server Disconnected, returning home.');
        this.props.navigate('login');
        // automatically try to reconnect on connection loss

        }
    }
    chooseBattle(type){
        this.props.onSelection(this.ws, type);
    }
    send(model){
        this.ws.send(model);
    }
    render(){
        if(!this.state.isConnected){
            return(
                <div><h2>Connecting...</h2></div>
            )
        }else{
            return this.renderConnected();
        }
    }
    renderConnected(){
        return(
            <div>
                <h2>Choose a battle mode!</h2>
                <br/>
                <div>
                    <button onClick={() => this.chooseBattle('ai')}>AI Battle</button>
                    <br/>
                    <button onClick={() => this.chooseBattle('mp')}>MultiPlayer Battle</button>
                </div>
            </div>
        );
    };
}