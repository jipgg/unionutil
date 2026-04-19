## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.Field()
       cmp       byte ptr [rdi+18],1
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 11
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsT()
       push      rbp
       mov       rbp,rsp
       movzx     eax,byte ptr [rdi+18]
       dec       eax
       jne       short M00_L02
M00_L00:
       mov       eax,1
M00_L01:
       pop       rbp
       ret
M00_L02:
       cmp       eax,2
       ja        short M00_L03
       mov       ecx,1
       bt        ecx,eax
       jb        short M00_L00
M00_L03:
       xor       eax,eax
       jmp       short M00_L01
; Total bytes of code 38
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsIndex()
       cmp       byte ptr [rdi+18],1
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 11
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsTag_Dense()
       movzx     eax,byte ptr [rdi+18]
       dec       eax
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 13
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsTag_Sparse()
       push      rbp
       push      r15
       push      r14
       push      rbx
       push      rax
       lea       rbp,[rsp+20]
M00_L00:
       add       rdi,20
       movzx     eax,byte ptr [rdi+10]
       dec       eax
       jne       short M00_L01
       mov       eax,1
       add       rsp,8
       pop       rbx
       pop       r14
       pop       r15
       pop       rbp
       ret
M00_L01:
       cmp       eax,2
       ja        short M00_L02
       mov       eax,eax
       lea       rdi,[7F4126F7C140]
       mov       edi,[rdi+rax*4]
       lea       rcx,[M00_L00]
       add       rdi,rcx
       jmp       rdi
M00_L02:
       lea       rbx,[rdi+10]
       mov       rdi,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       r15,rax
       mov       edi,1FB
       mov       rsi,7F41273552E0
       call      qword ptr [7F4126F0F2B8]
       mov       r14,rax
       movzx     edi,byte ptr [rbx]
       call      qword ptr [7F41279B4C90]; System.Number.UInt32ToDecStr(UInt32)
       mov       rsi,rax
       mov       rdi,r14
       call      qword ptr [7F4126F0D2A8]; System.String.Concat(System.String, System.String)
       mov       rsi,rax
       mov       rdi,r15
       call      qword ptr [7F41274356E0]
       mov       rdi,r15
       call      CORINFO_HELP_THROW
       int       3
       xor       eax,eax
       add       rsp,8
       pop       rbx
       pop       r14
       pop       r15
       pop       rbp
       ret
; Total bytes of code 171
```
```assembly
; System.Number.UInt32ToDecStr(UInt32)
       push      rbx
       mov       ebx,edi
       cmp       ebx,12C
       jae       short M01_L00
       mov       rdi,7F4127A8E350
       call      CORINFO_HELP_COUNTPROFILE32
       mov       edi,ebx
       pop       rbx
       jmp       qword ptr [7F41279B4CA8]; System.Number.UInt32ToDecStrForKnownSmallNumber(UInt32)
M01_L00:
       mov       rdi,7F4127A8E354
       call      CORINFO_HELP_COUNTPROFILE32
       mov       edi,ebx
       pop       rbx
       jmp       qword ptr [7F41279B4CF0]; System.Number.UInt32ToDecStr_NoSmallNumberCheck(UInt32)
; Total bytes of code 59
```
```assembly
; System.String.Concat(System.String, System.String)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rbx
       sub       rsp,18
       lea       rbp,[rsp+40]
       mov       rbx,rdi
       mov       r15,rsi
       test      rbx,rbx
       je        near ptr M02_L01
       mov       r14d,[rbx+8]
       test      r14d,r14d
       je        near ptr M02_L01
       test      r15,r15
       je        short M02_L00
       mov       r13d,[r15+8]
       test      r13d,r13d
       je        short M02_L00
       mov       r12d,r14d
       lea       esi,[r12+r13]
       test      esi,esi
       jl        near ptr M02_L04
       movsxd    rsi,esi
       mov       rdi,offset MT_System.String
       call      00007F41A54D5B00
       mov       [rbp-30],rax
       cmp       [rax],al
       lea       rcx,[rax+0C]
       mov       [rbp-38],rcx
       mov       rdi,rcx
       mov       edx,r14d
       add       rdx,rdx
       lea       rsi,[rbx+0C]
       call      qword ptr [7F4126F05800]; System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       mov       edi,r12d
       mov       rbx,[rbp-38]
       lea       rdi,[rbx+rdi*2]
       mov       edx,r13d
       add       rdx,rdx
       lea       rsi,[r15+0C]
       call      qword ptr [7F4126F05800]; System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       mov       rax,[rbp-30]
       add       rsp,18
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M02_L00:
       mov       rax,rbx
       add       rsp,18
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M02_L01:
       test      r15,r15
       je        short M02_L02
       mov       r13d,[r15+8]
       test      r13d,r13d
       sete      al
       movzx     eax,al
       test      eax,eax
       je        short M02_L03
M02_L02:
       mov       rax,7F4116600008
       add       rsp,18
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M02_L03:
       mov       rax,r15
       add       rsp,18
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M02_L04:
       call      qword ptr [7F41279BDFC8]
       int       3
; Total bytes of code 263
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsT_OpenGeneric()
       cmp       [rdi],dil
       lea       rsi,[rdi+8]
       mov       rdi,7F627B018AD8
       jmp       qword ptr [7F627AFD6D90]; IsCheck.CheckIs[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef)
; Total bytes of code 23
```
```assembly
; IsCheck.CheckIs[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.Int32, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef)
       push      rbp
       mov       rbp,rsp
       movzx     eax,byte ptr [rsi+10]
       dec       eax
       jne       short M01_L02
M01_L00:
       mov       eax,1
M01_L01:
       pop       rbp
       ret
M01_L02:
       cmp       eax,2
       ja        short M01_L03
       mov       ecx,1
       bt        ecx,eax
       jb        short M01_L00
M01_L03:
       xor       eax,eax
       jmp       short M01_L01
; Total bytes of code 38
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsT_OpenGeneric_Mismatch()
       cmp       [rdi],dil
       lea       rsi,[rdi+8]
       mov       rdi,7F4BF8418AD8
       jmp       qword ptr [7F4BF83D6D90]; IsCheck.CheckIs[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.Single, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef)
; Total bytes of code 23
```
```assembly
; IsCheck.CheckIs[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.Single, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef)
       push      rbp
       mov       rbp,rsp
       movzx     eax,byte ptr [rsi+10]
       dec       eax
       jne       short M01_L02
M01_L00:
       xor       eax,eax
M01_L01:
       pop       rbp
       ret
M01_L02:
       cmp       eax,2
       ja        short M01_L00
       mov       ecx,5
       bt        ecx,eax
       jb        short M01_L00
       mov       eax,1
       jmp       short M01_L01
; Total bytes of code 38
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; IsCheck.IsIndex_OpenGeneric()
       cmp       [rdi],dil
       lea       rsi,[rdi+8]
       mov       rdi,7F9ED47F9708
       mov       edx,1
       jmp       qword ptr [7F9ED47B6E98]; IsCheck.CheckIndex[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef, Byte)
; Total bytes of code 28
```
```assembly
; IsCheck.CheckIndex[[System.Int32, System.Private.CoreLib],[System.Single, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](DenseCase`3<Int32,Single,System.__Canon> ByRef, Byte)
       movzx     eax,byte ptr [rsi+10]
       movzx     ecx,dl
       cmp       eax,ecx
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 16
```

