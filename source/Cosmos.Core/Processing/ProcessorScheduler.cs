﻿using System;
using System.Collections.Generic;
using System.Text;
using IL2CPU.API.Attribs;

namespace Cosmos.Core.Processing
{
    public static unsafe class ProcessorScheduler
    {
        public static void Initialize()
        {
            var context = new ProcessContext.Context();
            context.type = ProcessContext.Context_Type.PROCESS;
            context.tid = ProcessContext.m_NextCID++;
            //context.name = "Boot";
            context.esp = 0;
            context.stacktop = 0;
            context.cr3 = 0;
            context.state = ProcessContext.Thread_State.ALIVE;
            context.old_state = ProcessContext.Thread_State.ALIVE;
            context.arg = 0;
            context.priority = 0;
            context.age = 0;
            context.parent = 0;
            ProcessContext.m_ContextList = context;
            ProcessContext.m_CurrentContext = context;

            IOPort counter0 = new IOPort(0x40);
            IOPort cmd = new IOPort(0x43);
            
            int divisor = 1193182 / 20;
            cmd.Byte = (0x06 | 0x30);
            counter0.Byte = (byte)divisor;
            counter0.Byte = (byte)(divisor >> 8);

            IOPort pA1 = new IOPort(0xA1);
            IOPort p21 = new IOPort(0xA1);
            pA1.Byte = 0x00;
            p21.Byte = 0x00;
        }

        public static void EntryPoint()
        {
            ProcessContext.m_CurrentContext.entry?.Invoke();
            ProcessContext.m_CurrentContext.paramentry?.Invoke(ProcessContext.m_CurrentContext.param);
            ProcessContext.m_CurrentContext.state = ProcessContext.Thread_State.DEAD;
            while(true) { } // remove from thread pool later
        }

        public static int interruptCount;

        public static void SwitchTask()
        {
            interruptCount++;
            if (ProcessContext.m_CurrentContext != null)
            {
                ProcessContext.m_CurrentContext.esp = INTs.mStackContext;
                if (ProcessContext.m_CurrentContext.next != null)
                {
                    ProcessContext.m_CurrentContext = ProcessContext.m_CurrentContext.next;
                }
                else
                {
                    ProcessContext.m_CurrentContext = ProcessContext.m_ContextList;
                }
                INTs.mStackContext = ProcessContext.m_CurrentContext.esp;
            }
            Global.PIC.EoiMaster();
            Global.PIC.EoiSlave();
        }
    }
}
