export class loginWithExternal{
    accessToken : string;
    userId : string;
    providers : string

    constructor(accessToken : string, userId : string, providers : string){
        this.accessToken = accessToken;
        this.userId = userId;
        this.providers = providers;
    }
}