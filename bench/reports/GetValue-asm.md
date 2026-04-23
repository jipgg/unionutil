## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; GetValue.FieldAccess()
       cmp       byte ptr [rdi+20],1
       jne       short M00_L00
       mov       rax,[rdi+28]
       mov       rdx,[rdi+30]
       ret
M00_L00:
       xor       eax,eax
       xor       edx,edx
       ret
; Total bytes of code 20
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; GetValue.ValueProperty()
       push      rbp
       mov       rbp,rsp
       cmp       [rdi],dil
       add       rdi,8
       mov       rsi,offset MT_BasicUnion<_16B, System.Double, System.Int32[], System.Double[]>
       call      qword ptr [7F8D3FDC6F58]; BasicUnion`4[[_16B, Bench],[System.Double, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]].get_Value()
       test      rax,rax
       je        short M00_L00
       mov       rdx,offset MT__16B
       cmp       [rax],rdx
       jne       short M00_L00
       mov       rcx,[rax+8]
       mov       rdx,[rax+10]
       mov       rax,rcx
       pop       rbp
       ret
M00_L00:
       xor       eax,eax
       xor       edx,edx
       pop       rbp
       ret
; Total bytes of code 66
```
```assembly
; BasicUnion`4[[_16B, Bench],[System.Double, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]].get_Value()
       push      rbp
       push      r15
       push      rbx
       sub       rsp,30
       lea       rbp,[rsp+40]
       xor       eax,eax
       mov       [rbp-38],rax
       vxorps    xmm8,xmm8,xmm8
       vmovdqu   ymmword ptr [rbp-30],ymm8
       mov       rbx,rdi
M01_L00:
       movzx     edi,byte ptr [rbx+18]
       dec       edi
       jne       short M01_L02
       mov       rdi,offset MT__16B
       call      CORINFO_HELP_NEWSFAST
       vmovups   xmm0,[rbx+20]
       vmovups   [rax+8],xmm0
M01_L01:
       add       rsp,30
       pop       rbx
       pop       r15
       pop       rbp
       ret
M01_L02:
       cmp       edi,3
       ja        short M01_L03
       mov       edi,edi
       lea       rax,[7F8D3F38CD60]
       mov       eax,[rax+rdi*4]
       lea       rcx,[M01_L00]
       add       rax,rcx
       jmp       rax
M01_L03:
       lea       rdi,[rbp-38]
       mov       esi,0F
       mov       edx,1
       call      qword ptr [7F8D3F317990]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       r15d,[rbp-28]
       cmp       r15d,[rbp-18]
       jbe       short M01_L05
       jmp       short M01_L04
       mov       rdi,offset MT_System.Double
       call      CORINFO_HELP_NEWSFAST
       vmovsd    xmm0,qword ptr [rbx+10]
       vmovsd    qword ptr [rax+8],xmm0
       jmp       short M01_L01
       mov       rax,[rbx]
       jmp       short M01_L01
       mov       rax,[rbx+8]
       jmp       short M01_L01
M01_L04:
       call      qword ptr [7F8D3F6E7ED0]
       int       3
M01_L05:
       mov       rdi,[rbp-20]
       mov       esi,r15d
       lea       rdi,[rdi+rsi*2]
       mov       esi,[rbp-18]
       sub       esi,r15d
       cmp       esi,0F
       jb        short M01_L06
       vmovups   xmm0,[7F8D3F38CD70]
       vmovups   [rdi],xmm0
       mov       rsi,77002000780065
       mov       [rdi+10],rsi
       mov       dword ptr [rdi+18],730061
       mov       word ptr [rdi+1C],20
       mov       edi,[rbp-28]
       add       edi,0F
       mov       [rbp-28],edi
       jmp       short M01_L07
M01_L06:
       lea       rdi,[rbp-38]
       mov       rsi,7F8D2E60A210
       call      qword ptr [7F8D3FD5D470]
M01_L07:
       movzx     esi,byte ptr [rbx+18]
       lea       rdi,[rbp-38]
       call      qword ptr [7F8D3FDC6F70]
       mov       rdi,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rdi,[rbp-38]
       call      qword ptr [7F8D3F3179C0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rsi,rax
       mov       rdi,rbx
       call      qword ptr [7F8D3F8457E8]
       mov       rdi,rbx
       call      CORINFO_HELP_THROW
       int       3
; Total bytes of code 337
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; GetValue.TryGetValue()
       add       rdi,8
       cmp       byte ptr [rdi+18],1
       jne       short M00_L00
       mov       rax,[rdi+20]
       mov       rdx,[rdi+28]
       ret
M00_L00:
       xor       eax,eax
       xor       edx,edx
       ret
; Total bytes of code 24
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; GetValue.TryGetValueT()
       push      rbp
       push      r15
       push      rbx
       sub       rsp,10
       lea       rbp,[rsp+20]
       xor       eax,eax
       mov       [rbp-20],rax
       mov       [rbp-18],rax
       mov       rbx,rdi
       mov       rdi,offset MT_BasicUnion<_16B, System.Double, System.Int32[], System.Double[]>
       call      CORINFO_HELP_NEWSFAST
       mov       r15,rax
       lea       rsi,[rbx+8]
       lea       rdi,[r15+8]
       call      CORINFO_HELP_ASSIGN_BYREF
       call      CORINFO_HELP_ASSIGN_BYREF
       mov       ecx,4
       rep movsq
       mov       rdi,r15
       mov       rsi,offset MT_UnionUtil.IUnion
       mov       rdx,7F47FE207938
       call      System.Runtime.CompilerServices.VirtualDispatchHelpers.VirtualFunctionPointer(System.Object, IntPtr, IntPtr)
       lea       rsi,[rbp-20]
       mov       rdi,r15
       call      rax
       test      eax,eax
       je        short M00_L00
       mov       rax,[rbp-20]
       mov       rdx,[rbp-18]
       add       rsp,10
       pop       rbx
       pop       r15
       pop       rbp
       ret
M00_L00:
       xor       eax,eax
       xor       edx,edx
       add       rsp,10
       pop       rbx
       pop       r15
       pop       rbp
       ret
; Total bytes of code 141
```
```assembly
; System.Runtime.CompilerServices.VirtualDispatchHelpers.VirtualFunctionPointer(System.Object, IntPtr, IntPtr)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rbx
       lea       rbp,[rsp+28]
       mov       rax,[rdi]
       mov       ecx,esi
       rol       ecx,5
       add       ecx,eax
       mov       r8d,edx
       ror       r8d,5
       add       ecx,r8d
       mov       r8,7F3C4D800810
       mov       r8,[r8]
       mov       r8,[r8+8]
       movsxd    r9,ecx
       mov       r10,9E3779B97F4A7C15
       imul      r9,r10
       movzx     r10d,byte ptr [r8+10]
       shrx      r9,r9,r10
       xor       r10d,r10d
M01_L00:
       lea       r11d,[r9+1]
       movsxd    r11,r11d
       imul      r11,30
       lea       r11,[r8+r11+10]
       mov       ebx,[r11]
       mov       r15d,[r11+8]
       mov       r14,[r11+10]
       mov       r13,[r11+18]
       mov       r12,[r11+20]
       cmp       ecx,r15d
       jne       short M01_L01
       mov       r15,rax
       sub       r15,r14
       mov       r14,rsi
       sub       r14,r13
       or        r15,r14
       mov       r14,rdx
       sub       r14,r12
       or        r15,r14
       je        short M01_L03
M01_L01:
       test      ebx,ebx
       je        short M01_L02
       inc       r10d
       add       r9d,r10d
       mov       r11d,[r8+8]
       add       r11d,0FFFFFFFE
       and       r9d,r11d
       cmp       r10d,8
       jl        short M01_L00
M01_L02:
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       jmp       qword ptr [7F47FE154240]; System.Runtime.CompilerServices.VirtualDispatchHelpers.VirtualFunctionPointerSlow(System.Object, IntPtr, IntPtr)
M01_L03:
       mov       rax,[r11+28]
       and       ebx,0FFFFFFFE
       cmp       ebx,[r11]
       jne       short M01_L02
       pop       rbx
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
; Total bytes of code 214
```

## .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3 (Job: .NET 10.0(Runtime=.NET 10.0))

```assembly
; GetValue.TryGetValueTGeneric()
       push      rbp
       mov       rbp,rsp
       add       rdi,8
       movzx     eax,byte ptr [rdi+18]
       lea       edx,[rax-1]
       test      edx,edx
       jne       short M00_L03
M00_L00:
       cmp       eax,1
       jne       short M00_L04
       mov       rax,[rdi+20]
       mov       rdx,[rdi+28]
       mov       ecx,1
M00_L01:
       test      ecx,ecx
       je        short M00_L06
M00_L02:
       pop       rbp
       ret
M00_L03:
       cmp       edx,3
       ja        short M00_L05
       mov       ecx,1
       bt        ecx,edx
       jb        short M00_L00
       jmp       short M00_L05
M00_L04:
       xor       eax,eax
       xor       edx,edx
       xor       ecx,ecx
       jmp       short M00_L01
M00_L05:
       xor       eax,eax
       xor       edx,edx
       xor       ecx,ecx
       jmp       short M00_L01
M00_L06:
       xor       eax,eax
       xor       edx,edx
       jmp       short M00_L02
; Total bytes of code 82
```

