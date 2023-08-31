import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from "@angular/router";

export class CustomRouteReuseStartegy implements RouteReuseStrategy
{
     shouldDetach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }
     store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle): void {
        // throw new Error("Method not implemented.");
    }
     shouldAttach(route: ActivatedRouteSnapshot): boolean {
       return false;
    }
     retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle {
        return null;;
    }
     shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
        return false;;
    }
    
}