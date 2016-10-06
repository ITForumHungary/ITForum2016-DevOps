import { Component } from "@angular/core";
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Observable }     from "rxjs/Observable";

@Component({
    selector: "home",
    template: require("./home.component.html"),
    styles: [require("./home.component.css")]
})
export class HomeComponent {
    public last: string;

    constructor(private http: Http) {
        this.last = "";
    }

    private headers = new Headers({ "Content-Type": "application/json" });
    private options = new RequestOptions({ headers: this.headers });

    vote(choice: string) {
        this.last = choice;
        this.http.post("/api/vote/submit", JSON.stringify({ "choice": choice }), this.options).subscribe(_ => {}, error => console.log(error));
    }
}
