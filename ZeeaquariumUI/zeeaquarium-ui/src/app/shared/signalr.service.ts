import { Injectable } from "@angular/core";
import * as signalR from "@aspnet/signalr";
import { BehaviorSubject, Observable } from "rxjs";
import { IMeasurement } from "../app.component";

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection;

    public temperature$ = new BehaviorSubject<IMeasurement>({
        name: 'Temperatuur', 
        value: '', 
        unit: '\u00B0C', 
        timeStamp: new Date().valueOf()
    });

    public constructor() {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('https://zeeaquarium-streamingapp.azurewebsites.net/data')
            .build();

        this.hubConnection.on('broadcastMessage', (name, message) => {
            if (name === 'temperature') {
                this.temperature$.next({
                    name: 'Temperatuur', 
                    value: message, 
                    unit: '\u00B0C', 
                    timeStamp: new Date().valueOf()
                });
            }
        });
    }

    public startConnection = () => {
        if (!this.isConnected()) {
            this.hubConnection
                .start()
                .then(() => console.log('Connection started'))
                .catch(err => console.log('Error while starting connection: ' + err));
        }
    }

    public getTemperature(): Observable<IMeasurement> {
        return this.temperature$.asObservable();
    }

    public isConnecting(): boolean {
        return this.hubConnection.state === 0;
    }

    public isConnected(): boolean {
        return this.hubConnection.state === 1;
    }

    public isReconnecting(): boolean {
        return this.hubConnection.state === 2;
    }

    public isDisconnected(): boolean {
        return this.hubConnection.state === 4;
    }
}