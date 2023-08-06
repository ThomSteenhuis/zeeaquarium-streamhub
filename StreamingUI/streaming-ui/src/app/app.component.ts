import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { interval, merge, of, Subscription } from 'rxjs';
import { catchError, mergeMap, tap, timeout } from 'rxjs/operators';

import { SignalRService } from './shared/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'streaming-ui';

  private subscriptions = new Subscription();

  constructor(
    public signalRService: SignalRService,
    public http: HttpClient
  ) {}

  public ngOnInit(): void {
    this.subscriptions.add(merge(of(''), interval(5000)).pipe(
      tap(() => this.signalRService.startConnection())
    ).subscribe());

    this.subscriptions.add(
      interval(1000).pipe(
        mergeMap(() => this.http.get('http://localhost:5000/temperature')),
        timeout(5000),
        catchError(error => {
          console.log(error);

          return of('');
        }),
        tap(temperature => {
          if (temperature !== '') {
            this.signalRService.sendMessage('temperature', temperature.toString());
          }
        })
      ).subscribe());
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
