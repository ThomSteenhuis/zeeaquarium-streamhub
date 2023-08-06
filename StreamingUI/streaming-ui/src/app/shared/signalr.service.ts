import { Injectable } from "@angular/core";
import * as signalR from "@aspnet/signalr";

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection;

    public constructor() {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('https://zeeaquarium-streamingapp.azurewebsites.net/data')
            .build();
    }

    public startConnection = () => {      
        if (!this.isConnected()) {
            this.hubConnection
                .start()
                .then(() => console.log('Connection started'))
                .catch(err => console.log('Error while starting connection: ' + err));
        }
    }

    public isConnected(): boolean {
        return this.hubConnection.state === 1;
    }

    public sendMessage(name: string, message: string): void {
        this.hubConnection.invoke('broadcastMessage', name, message);
    }
}