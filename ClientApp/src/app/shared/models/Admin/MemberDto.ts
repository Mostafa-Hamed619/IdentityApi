export interface MemberDto{
    id : string;
    firstName : string;
    lastName : string;
    userName : string;
    dateCreated :Date;
    isLocked : boolean;
    role : string[];
}