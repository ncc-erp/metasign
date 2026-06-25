import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { AppConsts } from "@shared/AppConsts";
import { Subject, Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class ContractSignalrService {
  private hubConnection: HubConnection;
  private contractUpdatedSource = new Subject<number>();

  // Expose the event stream as an Observable
  contractUpdated$: Observable<number> = this.contractUpdatedSource.asObservable();

  constructor() {}

  /**
   * Initializes the SignalR hub connection and joins the specific contract group.
   * @param contractId The ID of the contract to listen for updates.
   */
  init(contractId: number): void {
    const token = abp.auth.getToken();
    const url = AppConsts.remoteServiceBaseUrl + "/signalr-contract";

    // abp.log.debug(`Initializing SignalR connection to: ${url}`);

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(url, token ? { accessTokenFactory: () => token } : {})
      .build();

    this.hubConnection
      .start()
      .then(() => {
        // abp.log.debug(`SignalR connected successfully. Joining contract group: Contract-${contractId}`);
        this.hubConnection.send("JoinContract", contractId);
      })
      .catch((err) => {
        abp.log.error("SignalR connection start failed: " + err);
      });

    this.hubConnection.on("ContractUpdated", (id: number) => {
      // abp.log.debug(`Received ContractUpdated SignalR event for contractId: ${id}`);
      this.contractUpdatedSource.next(id);
    });
  }

  /**
   * Stops the SignalR hub connection and cleans up resources.
   */
  stop(): void {
    if (this.hubConnection) {
      // abp.log.debug("Stopping SignalR connection...");
      this.hubConnection.stop();
      this.hubConnection = null;
    }
  }
}
