import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { adminGuard } from '../shared/guards/admin.guard';
import { AddEditMemberComponent } from './add-edit-member/add-edit-member.component';

const routes : Routes = [

{
  path:'',
  runGuardsAndResolvers:"always",
  canActivate:[adminGuard],
  children:[
    {path:'',component:AdminComponent},
    {path:'admin/add-edit-member',component:AddEditMemberComponent},
    {path:'admin/add-edit-member/:id',component:AddEditMemberComponent}
  ]
}
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports:[
    RouterModule
  ]
})
export class AdminRoutingModule { }
